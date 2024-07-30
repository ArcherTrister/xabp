using X.Abp.IdentityServer.Pro.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace X.Abp.IdentityServer.Pro;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpIdentityServerEntityFrameworkCoreTestModule)
    )]
public class ProDomainTestModule : AbpModule
{

}
