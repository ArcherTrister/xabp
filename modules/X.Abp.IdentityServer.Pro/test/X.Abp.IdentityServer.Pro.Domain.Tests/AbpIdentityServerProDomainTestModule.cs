using Volo.Abp.Modularity;

using X.Abp.IdentityServer.EntityFrameworkCore;

namespace X.Abp.IdentityServer;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpIdentityServerProEntityFrameworkCoreTestModule),
    typeof(AbpIdentityServerProTestBaseModule)
    )]
public class AbpIdentityServerProDomainTestModule : AbpModule
{

}
