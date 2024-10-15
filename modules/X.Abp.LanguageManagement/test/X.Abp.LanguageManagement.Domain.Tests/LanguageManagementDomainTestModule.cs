using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpLanguageManagementDomainModule),
    typeof(LanguageManagementTestBaseModule)
    )]
public class LanguageManagementDomainTestModule : AbpModule
{

}
