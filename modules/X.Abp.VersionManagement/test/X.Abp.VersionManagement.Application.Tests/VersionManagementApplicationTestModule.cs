using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement;

[DependsOn(
    typeof(VersionManagementApplicationModule),
    typeof(VersionManagementDomainTestModule)
    )]
public class VersionManagementApplicationTestModule : AbpModule
{

}
