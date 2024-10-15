using Volo.Abp.Modularity;

namespace X.Abp.FileManagement;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpFileManagementDomainModule),
    typeof(FileManagementTestBaseModule)
    )]
public class FileManagementDomainTestModule : AbpModule
{

}
