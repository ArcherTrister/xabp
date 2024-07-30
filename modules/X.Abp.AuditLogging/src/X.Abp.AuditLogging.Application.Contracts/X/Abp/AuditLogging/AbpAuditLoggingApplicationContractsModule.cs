// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Authorization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.AuditLogging;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule))]
public class AbpAuditLoggingApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAuditLoggingApplicationContractsModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AuditLoggingResource>()
                .AddVirtualJson("/X/Abp/AuditLogging/Localization/Resources");
        });
    }
}
