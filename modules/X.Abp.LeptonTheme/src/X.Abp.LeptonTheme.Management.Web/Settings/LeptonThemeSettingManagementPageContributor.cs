// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.Features;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

using X.Abp.LeptonTheme.Management.Features;
using X.Abp.LeptonTheme.Management.Localization;
using X.Abp.LeptonTheme.Management.Permissions;
using X.Abp.LeptonTheme.Management.Web.Pages.LeptonThemeManagement.Components.LeptonThemeSettingGroup;

namespace X.Abp.LeptonTheme.Management.Web.Settings
{
    public class LeptonThemeSettingManagementPageContributor : ISettingPageContributor
    {
        public async Task ConfigureAsync(SettingPageCreationContext context)
        {
            var featureChecker = context.ServiceProvider.GetRequiredService<IFeatureChecker>();

            if (!await featureChecker.IsEnabledAsync(LeptonThemeManagementFeatures.Enable))
            {
                return;
            }

            if (!await CheckPermissionsInternalAsync(context))
            {
                return;
            }

            var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<LeptonThemeManagementResource>>();
            context.Groups.Add(
                new SettingPageGroup(
                    "Volo.Abp.LeptonThemeManagement",
                    l["Menu:LeptonThemeSettings"],
                    typeof(LeptonThemeSettingGroupViewComponent)));
        }

        public async Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
        {
            return await CheckPermissionsInternalAsync(context);
        }

        private static async Task<bool> CheckPermissionsInternalAsync(SettingPageCreationContext context)
        {
            var authService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();

            return await authService.IsGrantedAsync(LeptonThemeManagementPermissions.Settings.SettingsGroup);
        }
    }
}
