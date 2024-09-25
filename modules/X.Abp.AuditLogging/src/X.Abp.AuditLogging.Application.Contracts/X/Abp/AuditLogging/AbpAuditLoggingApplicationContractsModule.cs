// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application;
using Volo.Abp.AuditLogging;
using Volo.Abp.Authorization;
using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace X.Abp.AuditLogging;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule))]
public class AbpAuditLoggingApplicationContractsModule : AbpModule
{
}
