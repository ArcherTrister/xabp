// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

using X.Abp.LeptonTheme.Management.Localization;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLocalizationModule),
        typeof(AbpValidationModule),
        typeof(AbpFeaturesModule))]
    public class AbpLeptonThemeManagementDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpLeptonThemeManagementDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<LeptonThemeManagementResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/X/Abp/LeptonTheme/Management/Localization/Resources");
            });
        }
    }
}
