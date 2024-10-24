// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace X.Abp.OpenIddict;

[DependsOn(typeof(AbpOpenIddictProDomainSharedModule), typeof(AbpOpenIddictDomainModule))]
public class AbpOpenIddictProDomainModule : AbpModule
{
}
