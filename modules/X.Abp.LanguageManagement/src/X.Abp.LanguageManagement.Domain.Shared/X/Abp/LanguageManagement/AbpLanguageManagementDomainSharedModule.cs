// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.LanguageManagement.Localization;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpLocalizationModule),
    typeof(AbpFeaturesModule),
    typeof(AbpValidationModule))]
public class AbpLanguageManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpLanguageManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
            .Add<LanguageManagementResource>("en")
            .AddVirtualJson("/X/Abp/LanguageManagement/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.LanguageManagement", typeof(LanguageManagementResource));
        });
    }
}
