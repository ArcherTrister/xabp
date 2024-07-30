// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.TextTemplateManagement.Localization;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule))]
public class AbpTextTemplateManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<TextTemplateManagementResource>("en")
                .AddVirtualJson("/X/Abp/TextTemplateManagement/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.TextTemplateManagement", typeof(TextTemplateManagementResource));
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpTextTemplateManagementDomainSharedModule>();
        });
    }
}
