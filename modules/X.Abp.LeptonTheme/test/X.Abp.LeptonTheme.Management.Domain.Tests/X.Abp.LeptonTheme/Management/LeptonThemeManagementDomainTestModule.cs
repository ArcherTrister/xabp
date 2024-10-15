using Volo.Abp.Modularity;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
    typeof(AbpLeptonThemeManagementDomainModule),
    typeof(LeptonThemeManagementTestBaseModule)
        )]
    public class LeptonThemeManagementDomainTestModule : AbpModule
    {
        
    }
}
