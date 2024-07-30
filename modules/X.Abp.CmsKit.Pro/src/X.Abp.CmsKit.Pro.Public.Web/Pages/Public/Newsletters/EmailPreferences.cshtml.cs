// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.CmsKit.Newsletters.Helpers;
using X.Abp.CmsKit.Public.Newsletters;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Newsletters;

public class EmailPreferencesModel : AbpPageModel
{
    protected INewsletterRecordPublicAppService NewsletterRecordPublicAppService { get; }

    protected SecurityCodeProvider SecurityCodeProvider { get; }

    public List<NewsletterPreferenceDetailsViewModel> NewsletterPreferenceDetailsViewModels { get; set; }

    public string EmailAddress { get; set; }

    public EmailPreferencesModel(
      INewsletterRecordPublicAppService newsletterRecordPublicAppService,
      SecurityCodeProvider securityCodeProvider)
    {
        NewsletterRecordPublicAppService = newsletterRecordPublicAppService;
        SecurityCodeProvider = securityCodeProvider;
        NewsletterPreferenceDetailsViewModels = new List<NewsletterPreferenceDetailsViewModel>();
    }

    public async Task<IActionResult> OnGetAsync(string emailAddress, string securityCode)
    {
        if (securityCode != SecurityCodeProvider.GetSecurityCode(emailAddress))
        {
            return Unauthorized();
        }

        EmailAddress = emailAddress;
        foreach (var preferenceDetailsDto in await NewsletterRecordPublicAppService.GetNewsletterPreferencesAsync(emailAddress))
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
}
