using Volo.Abp.Modularity;

namespace X.Abp.LeptonTheme.Management
{
    [DependsOn(
        typeof(LeptonThemeManagementApplicationModule),
        typeof(LeptonThemeManagementDomainTestModule)
        )]
    public class LeptonThemeManagementApplicationTestModule : AbpModule
    {

    }
}
