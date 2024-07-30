using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(LanguageManagementApplicationModule),
    typeof(LanguageManagementDomainTestModule)
    )]
public class LanguageManagementApplicationTestModule : AbpModule
{

}
