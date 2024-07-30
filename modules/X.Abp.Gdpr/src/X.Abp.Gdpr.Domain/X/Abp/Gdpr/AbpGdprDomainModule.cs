// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Domain;
using Volo.Abp.Gdpr;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpGdprAbstractionsModule),
    typeof(AbpGdprDomainSharedModule),
    typeof(AbpDddDomainModule))]
public class AbpGdprDomainModule : AbpModule
{
}
