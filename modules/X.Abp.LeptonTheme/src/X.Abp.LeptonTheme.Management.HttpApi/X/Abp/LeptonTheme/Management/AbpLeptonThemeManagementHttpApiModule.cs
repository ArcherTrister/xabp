// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Localization.Resources.AbpUi;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using X.Abp.LeptonTheme.Management.Localization;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLeptonThemeManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class AbpLeptonThemeManagementHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpLeptonThemeManagementHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<LeptonThemeManagementResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
