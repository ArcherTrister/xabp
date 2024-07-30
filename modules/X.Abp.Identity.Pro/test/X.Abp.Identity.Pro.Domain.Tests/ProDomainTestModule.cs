using Volo.Abp.Identity.Pro.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Identity.Pro;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(ProEntityFrameworkCoreTestModule)
    )]
public class ProDomainTestModule : AbpModule
{

}
