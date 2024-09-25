// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz;

[DependsOn(
    typeof(AbpQuartzModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule))]
public class AbpQuartzDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpQuartzDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<QuartzResource>("en")
                .AddVirtualJson("/X/Abp/Quartz/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.Quartz", typeof(QuartzResource));
        });
    }
}
