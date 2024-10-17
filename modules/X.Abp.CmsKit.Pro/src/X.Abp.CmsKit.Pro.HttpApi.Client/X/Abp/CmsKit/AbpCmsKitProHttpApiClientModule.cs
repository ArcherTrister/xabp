// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Modularity;
using Volo.CmsKit;

using X.Abp.CmsKit.Admin;
using X.Abp.CmsKit.Public;

namespace X.Abp.CmsKit;

[DependsOn(
    typeof(CmsKitHttpApiClientModule),
    typeof(AbpCmsKitProApplicationContractsModule),
    typeof(AbpCmsKitProAdminHttpApiClientModule),
    typeof(AbpCmsKitProPublicHttpApiClientModule))]
public class AbpCmsKitProHttpApiClientModule : AbpModule
{
}
