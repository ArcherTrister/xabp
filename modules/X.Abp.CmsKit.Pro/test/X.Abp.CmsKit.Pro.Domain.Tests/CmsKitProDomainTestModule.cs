using Volo.Abp.Modularity;

using X.Abp.CmsKit.Pro.EntityFrameworkCore;

namespace X.Abp.CmsKit.Pro;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(CmsKitProEntityFrameworkCoreTestModule),
    typeof(CmsKitProTestBaseModule)
    )]
public class CmsKitProDomainTestModule : AbpModule
{

}
