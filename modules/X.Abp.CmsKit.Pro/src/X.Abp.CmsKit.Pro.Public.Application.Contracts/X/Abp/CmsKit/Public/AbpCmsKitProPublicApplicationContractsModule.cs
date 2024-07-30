// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.CmsKit.Public;

namespace X.Abp.CmsKit.Public;

[DependsOn(
    typeof(CmsKitProDomainSharedModule),
    typeof(AbpCachingModule),
    typeof(CmsKitPublicApplicationContractsModule))]
public class AbpCmsKitProPublicApplicationContractsModule : AbpModule
{
}
