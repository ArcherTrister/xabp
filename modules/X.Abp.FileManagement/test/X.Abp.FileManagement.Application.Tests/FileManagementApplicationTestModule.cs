using Volo.Abp.Modularity;

namespace X.Abp.FileManagement;

[DependsOn(
    typeof(FileManagementApplicationModule),
    typeof(FileManagementDomainTestModule)
    )]
public class FileManagementApplicationTestModule : AbpModule
{

}
