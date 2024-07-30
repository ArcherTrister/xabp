// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.CmsKit.Public.Newsletters;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Newsletters;

public class SuccessModalModel : CmsKitProPublicPageModel
{
    public string NormalizedSource;
    public List<string> AdditionalPreferences;
    public List<string> DisplayAdditionalPreferences;

    [BindProperty(SupportsGet = true)]
    [Required]
    public string EmailAddress { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required]
    public string Preference { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required]
    public string Source { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required]
    public string SourceUrl { get; set; }

    [Required]
    [BindProperty(SupportsGet = true)]
    public bool RequestAdditionalPreferences { get; set; }

    protected INewsletterRecordPublicAppService NewsletterRecordPublicAppService { get; }

    public SuccessModalModel(INewsletterRecordPublicAppService newsletterRecordPublicAppService)
    {
        NewsletterRecordPublicAppService = newsletterRecordPublicAppService;
    }

    public async Task OnGetAsync()
    {
        var newsletterEmailOptionsDto = await NewsletterRecordPublicAppService.GetOptionByPreferenceAsync(Preference);
        AdditionalPreferences = newsletterEmailOptionsDto.AdditionalPreferences;
        DisplayAdditionalPreferences = newsletterEmailOptionsDto.DisplayAdditionalPreferences;
        NormalizedSource = Source.Replace('.', '_');
    }

    public async Task OnPostAsync()
    {
        var newsletterEmailOptionsDto = await NewsletterRecordPublicAppService.GetOptionByPreferenceAsync(Preference);
        AdditionalPreferences = newsletterEmailOptionsDto.AdditionalPreferences;
        DisplayAdditionalPreferences = newsletterEmailOptionsDto.DisplayAdditionalPreferences;
        NormalizedSource = Source.Replace('.', '_');
    }
}
