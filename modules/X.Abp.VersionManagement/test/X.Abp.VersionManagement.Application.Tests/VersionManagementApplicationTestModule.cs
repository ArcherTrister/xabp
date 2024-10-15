using Volo.Abp.Modularity;

namespace X.Abp.VersionManagement;

[DependsOn(
    typeof(AbpVersionManagementApplicationModule),
    typeof(VersionManagementDomainTestModule)
    )]
public class VersionManagementApplicationTestModule : AbpModule
{

}
