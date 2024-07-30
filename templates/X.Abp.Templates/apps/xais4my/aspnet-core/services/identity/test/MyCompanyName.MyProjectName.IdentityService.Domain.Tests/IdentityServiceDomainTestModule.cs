using MyCompanyName.MyProjectName.IdentityService.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.IdentityService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(IdentityServiceEntityFrameworkCoreTestModule)
    )]
public class IdentityServiceDomainTestModule : AbpModule
{

}
