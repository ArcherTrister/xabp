using Volo.Abp.Modularity;

namespace X.Abp.FileManagement;

[DependsOn(
    typeof(AbpFileManagementApplicationModule),
    typeof(FileManagementDomainTestModule)
    )]
public class FileManagementApplicationTestModule : AbpModule
{

}
