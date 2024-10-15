// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Localization;

using X.Abp.CmsKit.Newsletters.Helpers;
using X.Abp.CmsKit.Public.Newsletters;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Newsletters;

public class EmailPreferencesModel : AbpPageModel
{
    protected INewsletterRecordPublicAppService NewsletterRecordPublicAppService { get; }

    protected SecurityCodeProvider SecurityCodeProvider { get; }

    public List<NewsletterPreferenceDetailsViewModel> NewsletterPreferenceDetailsViewModels { get; set; }

    public string EmailAddress { get; set; }

    public ILocalizableString PrivacyPolicyConfirmationMessage { get; set; }

    public string Source { get; set; }

    public string SecurityCode { get; set; }

    public NewsletterPreferencesManagementOptions NewsletterPreferencesManagementOption { get; set; }

    public EmailPreferencesModel(
    INewsletterRecordPublicAppService newsletterRecordPublicAppService,
    SecurityCodeProvider securityCodeProvider,
    IOptions<NewsletterPreferencesManagementOptions> newsletterPreferencesManagementOptions)
    {
        NewsletterRecordPublicAppService = newsletterRecordPublicAppService;
        SecurityCodeProvider = securityCodeProvider;
        NewsletterPreferenceDetailsViewModels = new List<NewsletterPreferenceDetailsViewModel>();
        NewsletterPreferencesManagementOption = newsletterPreferencesManagementOptions.Value;
    }

    public virtual async Task<IActionResult> OnGetAsync(string emailAddress, string securityCode)
    {
        SetSourceAndPrivacyMessage();
        if (securityCode.IsNullOrWhiteSpace() && emailAddress.IsNullOrWhiteSpace() && CurrentUser.IsAuthenticated && !CurrentUser.Email.IsNullOrWhiteSpace())
        {
            EmailAddress = CurrentUser.Email;
            SecurityCode = SecurityCodeProvider.GetSecurityCode(EmailAddress);
        }
        else
        {
            if (!CurrentUser.IsAuthenticated && (securityCode.IsNullOrWhiteSpace() || emailAddress.IsNullOrWhiteSpace()))
            {
                return Redirect("/Account/Login?ReturnUrl=" + HttpContext.Request.Path.Value);
            }

            if (emailAddress.IsNullOrWhiteSpace() || securityCode.IsNullOrWhiteSpace())
            {
                return Unauthorized();
            }

            if (securityCode != SecurityCodeProvider.GetSecurityCode(emailAddress))
            {
                return Unauthorized();
            }

            EmailAddress = emailAddress;
            SecurityCode = securityCode;
        }

        foreach (NewsletterPreferenceDetailsDto preferenceDetailsDto in await NewsletterRecordPublicAppService.GetNewsletterPreferencesAsync(EmailAddress))
        {
            NewsletterPreferenceDetailsViewModels.Add(new NewsletterPreferenceDetailsViewModel()
            {
                Preference = preferenceDetailsDto.Preference,
                DisplayPreference = preferenceDetailsDto.DisplayPreference,
                DisplayDefinition = preferenceDetailsDto.Definition,
                IsSelectedByEmailAddress = preferenceDetailsDto.IsSelectedByEmailAddress
            });
        }

        return Page();
    }

    public void SetSourceAndPrivacyMessage()
    {
        PrivacyPolicyConfirmationMessage = NewsletterPreferencesManagementOption.PrivacyPolicyConfirmation;
        Source = NewsletterPreferencesManagementOption.Source;
    }
}
