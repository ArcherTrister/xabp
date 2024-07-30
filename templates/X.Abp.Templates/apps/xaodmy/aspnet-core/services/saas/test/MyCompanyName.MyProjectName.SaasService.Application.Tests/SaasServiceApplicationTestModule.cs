using MyCompanyName.MyProjectName.SaasService.Application;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationModule),
    typeof(SaasServiceDomainTestModule)
    )]
public class SaasServiceApplicationTestModule : AbpModule
{

}
