// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(AbpTextTemplateManagementDomainSharedModule),
#pragma warning disable CS0618 // 类型或成员已过时
    typeof(AbpTextTemplatingModule),
#pragma warning restore CS0618 // 类型或成员已过时
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule))]
public class AbpTextTemplateManagementDomainModule : AbpModule
{
}
