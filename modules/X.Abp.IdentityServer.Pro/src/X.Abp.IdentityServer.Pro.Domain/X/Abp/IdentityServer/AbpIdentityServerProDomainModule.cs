// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

using X.Abp.Identity;

namespace X.Abp.IdentityServer;

[DependsOn(
    typeof(AbpIdentityServerProDomainSharedModule),
    typeof(AbpIdentityServerDomainModule),
    typeof(AbpIdentityProDomainModule))]
public class AbpIdentityServerProDomainModule : AbpModule
{
}
