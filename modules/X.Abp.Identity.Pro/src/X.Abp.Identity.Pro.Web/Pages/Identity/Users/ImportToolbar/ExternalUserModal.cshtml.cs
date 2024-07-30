// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace X.Abp.Identity.Web.Pages.Identity.Users.ImportToolbar;

public class ExternalUserModal : IdentityUserModalPageModel
{
    protected IIdentityUserAppService IdentityUserAppService { get; }

    public List<SelectListItem> ExternalLoginProviderItems { get; set; }

    public List<ExternalLoginProviderDto> ExternalLoginProviders { get; set; }

    [BindProperty]
    public ExternalUserViewModel ExternalUser { get; set; }

    public ExternalUserModal(IIdentityUserAppService identityUserAppService)
    {
        IdentityUserAppService = identityUserAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ExternalUser = new ExternalUserViewModel();
        ExternalLoginProviders = await IdentityUserAppService.GetExternalLoginProvidersAsync();
        ExternalLoginProviderItems = ExternalLoginProviders.Select(x => new SelectListItem(x.Name, x.Name)).ToList();

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        await IdentityUserAppService.ImportExternalUserAsync(new ImportExternalUserInput
        {
            Provider = ExternalUser.Provider,
            UserNameOrEmailAddress = ExternalUser.UserNameOrEmailAddress,
            Password = ExternalUser.Password
        });

        return NoContent();
    }

    public class ExternalUserViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string UserNameOrEmailAddress { get; set; }

        public string Password { get; set; }
    }
}
