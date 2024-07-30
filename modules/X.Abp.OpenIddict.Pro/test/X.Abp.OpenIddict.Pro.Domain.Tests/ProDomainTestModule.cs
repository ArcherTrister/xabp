using Volo.Abp.OpenIddict.Pro.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.OpenIddict.Pro;

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
