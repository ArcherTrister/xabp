using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpLanguageManagementApplicationModule),
    typeof(LanguageManagementDomainTestModule)
    )]
public class LanguageManagementApplicationTestModule : AbpModule
{

}
