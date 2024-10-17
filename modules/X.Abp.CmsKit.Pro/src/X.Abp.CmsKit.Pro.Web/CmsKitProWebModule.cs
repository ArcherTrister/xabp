// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Modularity;
using Volo.CmsKit.Web;

using X.Abp.CmsKit.Pro.Admin.Web;
using X.Abp.CmsKit.Pro.Public.Web;

namespace X.Abp.CmsKit.Pro.Web
{
    [DependsOn(
        typeof(CmsKitWebModule),
        typeof(AbpCmsKitProApplicationContractsModule),
        typeof(AbpCmsKitProPublicWebModule),
        typeof(AbpCmsKitProAdminWebModule))]
    public class CmsKitProWebModule : AbpModule
    {
    }
}
