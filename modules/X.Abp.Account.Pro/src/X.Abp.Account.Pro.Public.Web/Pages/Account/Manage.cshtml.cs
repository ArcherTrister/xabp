// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using X.Abp.Account.Public.Web.ProfileManagement;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class ManageModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    public ProfileManagementPageCreationContext ProfileManagementPageCreationContext { get; private set; }

    protected ProfileManagementPageOptions Options { get; }

    public ManageModel(IOptions<ProfileManagementPageOptions> options)
    {
        Options = options.Value;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ProfileManagementPageCreationContext = new ProfileManagementPageCreationContext(LazyServiceProvider.LazyGetRequiredService<IServiceProvider>());

        foreach (var contributor in Options.Contributors)
        {
            await contributor.ConfigureAsync(ProfileManagementPageCreationContext);
        }

        if (ReturnUrl != null)
        {
            if (!Url.IsLocalUrl(ReturnUrl) &&
                !ReturnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/"), StringComparison.InvariantCultureIgnoreCase) &&
                !await AppUrlProvider.IsRedirectAllowedUrlAsync(ReturnUrl))
            {
                ReturnUrl = null;
            }
        }

        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
