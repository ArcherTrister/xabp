using Volo.Abp.Modularity;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(AbpLeptonThemeManagementApplicationModule),
        typeof(LeptonThemeManagementDomainTestModule)
        )]
    public class LeptonThemeManagementApplicationTestModule : AbpModule
    {

    }
}
