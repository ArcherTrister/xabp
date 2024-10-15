using Volo.Abp.Modularity;

namespace X.Abp.TextTemplateManagement;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpTextTemplateManagementDomainModule),
    typeof(TextTemplateManagementTestBaseModule)
    )]
public class TextTemplateManagementDomainTestModule : AbpModule
{

}
