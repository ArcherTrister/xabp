using X.Abp.Gdpr.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(GdprEntityFrameworkCoreTestModule)
    )]
public class GdprDomainTestModule : AbpModule
{

}
