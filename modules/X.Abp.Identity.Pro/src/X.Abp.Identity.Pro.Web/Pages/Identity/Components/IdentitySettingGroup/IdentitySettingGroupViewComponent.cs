// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Features;

using X.Abp.Identity.Features;

namespace X.Abp.Identity.Web.Pages.Identity.Components.IdentitySettingGroup;

public class IdentitySettingGroupViewComponent : AbpViewComponent
{
    protected IIdentitySettingsAppService IdentitySettingsAppService { get; }

    protected IFeatureChecker FeatureChecker { get; }

    public IdentitySettingGroupViewComponent(IIdentitySettingsAppService identitySettingsAppService, IFeatureChecker featureChecker)
    {
        ObjectMapperContext = typeof(AbpIdentityProWebModule);

        IdentitySettingsAppService = identitySettingsAppService;
        FeatureChecker = featureChecker;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var settingsViewModel = new IdentitySettingViewModel
        {
            IdentitySettings = await IdentitySettingsAppService.GetAsync()
        };

        if (await FeatureChecker.IsEnabledAsync(IdentityProFeature.EnableLdapLogin))
        {
            settingsViewModel.IdentityLdapSettings = await IdentitySettingsAppService.GetLdapAsync();
        }

        if (await FeatureChecker.IsEnabledAsync(IdentityProFeature.EnableOAuthLogin))
        {
            settingsViewModel.IdentityOAuthSettings = await IdentitySettingsAppService.GetOAuthAsync();
        }

        return View("~/Pages/Identity/Components/IdentitySettingGroup/Default.cshtml", settingsViewModel);
    }

    public class IdentitySettingViewModel
    {
        public IdentitySettingsDto IdentitySettings { get; set; }

        public IdentityLdapSettingsDto IdentityLdapSettings { get; set; }

        public IdentityOAuthSettingsDto IdentityOAuthSettings { get; set; }
    }
}
