// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.Newsletters.Helpers;
using X.Abp.CmsKit.Public.Emailing.Templates;

namespace X.Abp.CmsKit.Public.Newsletters;

public class NewsletterRecordPublicAppService : PublicAppService, INewsletterRecordPublicAppService
{
    protected INewsletterPreferenceDefinitionStore NewsletterPreferenceDefinitionStore { get; }

    protected INewsletterRecordRepository NewsletterRecordsRepository { get; }

    protected NewsletterRecordManager NewsletterRecordsManager { get; }

    protected IEmailSender EmailSender { get; }

    protected ITemplateRenderer TemplateRenderer { get; }

    protected SecurityCodeProvider SecurityCodeProvider { get; }

    protected IAppUrlProvider AppUrlProvider { get; }

    public NewsletterRecordPublicAppService(
      INewsletterPreferenceDefinitionStore newsletterPreferenceDefinitionStore,
      INewsletterRecordRepository newsletterRecordsRepository,
      NewsletterRecordManager newsletterRecordsManager,
      IEmailSender emailSender,
      ITemplateRenderer templateRenderer,
      SecurityCodeProvider securityCodeProvider,
      IAppUrlProvider appUrlProvider)
    {
        NewsletterPreferenceDefinitionStore = newsletterPreferenceDefinitionStore;
        NewsletterRecordsRepository = newsletterRecordsRepository;
        NewsletterRecordsManager = newsletterRecordsManager;
        EmailSender = emailSender;
        TemplateRenderer = templateRenderer;
        SecurityCodeProvider = securityCodeProvider;
        AppUrlProvider = appUrlProvider;
    }

    public virtual async Task CreateAsync(CreateNewsletterRecordInput input)
    {
        var newsletterRecord = await NewsletterRecordsRepository.FindByEmailAddressAsync(input.EmailAddress);
        if (newsletterRecord == null || !newsletterRecord.Preferences.Any(x => x.Preference == input.Preference) || input.AdditionalPreferences.Count >= 1)
        {
            await NewsletterRecordsManager.CreateOrUpdateAsync(input.EmailAddress, input.Preference, input.Source, input.SourceUrl, input.AdditionalPreferences);
            await SendEmailAsync(input.EmailAddress, NewsletterEmailStatus.Subscription);
        }
        else
        {
            return;
        }
    }

    public virtual async Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync(string emailAddress)
    {
        var newsletterRecord = await NewsletterRecordsRepository.FindByEmailAddressAsync(emailAddress);
        if (newsletterRecord == null)
        {
            return new List<NewsletterPreferenceDetailsDto>();
        }

        var preferences = await NewsletterRecordsManager.GetNewsletterPreferencesAsync();
        var preferenceDefinitionList = new List<NewsletterPreferenceDetailsDto>();
        foreach (var preference in preferences)
        {
            preferenceDefinitionList.Add(new NewsletterPreferenceDetailsDto()
            {
                Preference = preference.Preference,
                DisplayPreference = preference.DisplayPreference.Localize(StringLocalizerFactory),
                IsSelectedByEmailAddress = newsletterRecord.Preferences.Any(x => x.Preference == preference.Preference),
                Definition = preference.Definition.Localize(StringLocalizerFactory)
            });
        }

        return preferenceDefinitionList;
    }

    public virtual async Task UpdatePreferencesAsync(UpdatePreferenceRequestInput input)
    {
        if (SecurityCodeProvider.GetSecurityCode(input.EmailAddress) != input.SecurityCode)
        {
            return;
        }

        var newsletterRecord1 = await NewsletterRecordsRepository.FindByEmailAddressAsync(input.EmailAddress);
        if (newsletterRecord1 == null)
        {
            return;
        }

        if (input.PreferenceDetails.Any(x => x.IsEnabled))
        {
            var isPreferenceChanged = false;
            foreach (var preferenceDetail1 in input.PreferenceDetails)
            {
                var preferenceDetail = preferenceDetail1;
                var newsletterPreference = newsletterRecord1.Preferences.FirstOrDefault(x => x.Preference == preferenceDetail.Preference);
                if (newsletterPreference == null && preferenceDetail.IsEnabled)
                {
                    newsletterRecord1.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord1.Id, preferenceDetail.Preference, input.Source, input.SourceUrl, CurrentTenant.Id));
                    isPreferenceChanged = true;
                }
                else if (newsletterPreference != null && !preferenceDetail.IsEnabled)
                {
                    newsletterRecord1.RemovePreference(newsletterPreference.Id);
                    isPreferenceChanged = true;
                }
            }

            await NewsletterRecordsRepository.UpdateAsync(newsletterRecord1, false);
            if (!isPreferenceChanged)
            {
                return;
            }

            await SendEmailAsync(input.EmailAddress, NewsletterEmailStatus.UpdatePreference);
        }
        else
        {
            await NewsletterRecordsRepository.DeleteAsync(newsletterRecord1.Id, false);
            await SendEmailAsync(input.EmailAddress, NewsletterEmailStatus.DeleteSubscription);
        }
    }

    public virtual async Task<NewsletterEmailOptionsDto> GetOptionByPreferenceAsync(string preference)
    {
        var source = await NewsletterRecordsManager.GetNewsletterPreferencesAsync();
        var preferenceDefinition = source.FirstOrDefault(x => x.Preference == preference);
        var newsletterEmailOption = ObjectMapper.Map<NewsletterPreferenceDefinition, NewsletterEmailOptionsDto>(preferenceDefinition!);
        if (preferenceDefinition?.AdditionalPreferences == null || preferenceDefinition.AdditionalPreferences.Count == 0)
        {
            return newsletterEmailOption;
        }

        for (var index = 0; index < source.Count; ++index)
        {
            if (source[index].AdditionalPreferences != null)
            {
                foreach (var additionalPreference in source[index].AdditionalPreferences)
                {
                    var additionalPreferenceDefinition = source.FirstOrDefault(x => x.Preference == additionalPreference);
                    if (additionalPreferenceDefinition != null && additionalPreference != preference)
                    {
                        newsletterEmailOption.AdditionalPreferences.Add(additionalPreferenceDefinition.Preference);
                        newsletterEmailOption.DisplayAdditionalPreferences.Add(additionalPreferenceDefinition.DisplayPreference.Localize(StringLocalizerFactory));
                    }
                }
            }
        }

        newsletterEmailOption.PrivacyPolicyConfirmation = preferenceDefinition.PrivacyPolicyConfirmation.Localize(StringLocalizerFactory);
        newsletterEmailOption.AdditionalPreferences = newsletterEmailOption.AdditionalPreferences.Distinct().ToList();
        newsletterEmailOption.DisplayAdditionalPreferences = newsletterEmailOption.DisplayAdditionalPreferences.Distinct().ToList();
        return newsletterEmailOption;
    }

    private async Task SendEmailAsync(string emailAddress, NewsletterEmailStatus newsletterEmailStatus)
    {
        (var subject, var newsletterEmailBodyModel) = await CreateNewsletterEmailBodyAsync(emailAddress, newsletterEmailStatus);
        var body = await TemplateRenderer.RenderAsync(CmsKitEmailTemplates.NewsletterEmailTemplate, newsletterEmailBodyModel);
        await EmailSender.SendAsync(emailAddress, subject, body, true);
    }

    private async Task<(string Subject, NewsletterEmailBodyModel Body)> CreateNewsletterEmailBodyAsync(
      string emailAddress,
      NewsletterEmailStatus newsletterEmailStatus)
    {
        var appUrl = await AppUrlProvider.GetUrlOrNullAsync("MVCPublic");
        appUrl ??= await AppUrlProvider.GetUrlAsync("MVC");

        var securityCode = SecurityCodeProvider.GetSecurityCode(emailAddress);
        var subject = string.Empty;
        var newsletterEmailBodyModel = new NewsletterEmailBodyModel()
        {
            Root = appUrl.EnsureEndsWith('/', StringComparison.Ordinal),
            Text = L["NewsletterEmailFooterTemplateManageEmail"],
            Email = emailAddress,
            Code = securityCode
        };
        switch (newsletterEmailStatus)
        {
            case NewsletterEmailStatus.Subscription:
                newsletterEmailBodyModel.Title = L["NewsletterEmailTitle"];
                newsletterEmailBodyModel.Description = L["NewsletterEmailFooterCreateTemplateMessage"];
                subject = L["NewsletterEmailSubject"];
                break;
            case NewsletterEmailStatus.UpdatePreference:
                newsletterEmailBodyModel.Title = L["NewsletterUpdatePreferenceTitle"];
                newsletterEmailBodyModel.Description = L["NewsletterEmailFooterUpdateTemplateMessage"];
                subject = L["NewsletterUpdatePreferenceTitleSubject"];
                break;
            case NewsletterEmailStatus.DeleteSubscription:
                newsletterEmailBodyModel = new NewsletterEmailBodyModel
                {
                    Title = L["NewsletterDeleteSubscriptionTitle"]
                };
                subject = L["NewsletterDeleteSubscriptionSubject"];
                break;
        }

        return (subject, newsletterEmailBodyModel);
    }
}
