// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactor;

public class AccountProfileTwoFactorManagementGroupViewComponent : AbpViewComponent
{
    protected IProfileAppService ProfileAppService { get; }

    public AccountProfileTwoFactorManagementGroupViewComponent(IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new ChangeTwoFactorModel
        {
            TwoFactorEnabled = await ProfileAppService.GetTwoFactorEnabledAsync()
        };

        return View("~/Pages/Account/Components/ProfileManagementGroup/TwoFactor/Default.cshtml", model);
    }

    public class ChangeTwoFactorModel
    {
        public bool TwoFactorEnabled { get; set; }
    }
}
