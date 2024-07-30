using MyCompanyName.MyProjectName.AdministrationService;
using MyCompanyName.MyProjectName.AdministrationService.EntityFrameworkCore;
using MyCompanyName.MyProjectName.IdentityService;
using MyCompanyName.MyProjectName.IdentityService.EntityFramework;
using MyCompanyName.MyProjectName.ProductService;
using MyCompanyName.MyProjectName.ProductService.EntityFrameworkCore;
using MyCompanyName.MyProjectName.SaasService;
using MyCompanyName.MyProjectName.SaasService.EntityFramework;
using MyCompanyName.MyProjectName.Shared.Hosting;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.DbMigrator;

[DependsOn(
    typeof(MyProjectNameSharedHostingModule),
    typeof(IdentityServiceEntityFrameworkCoreModule),
    typeof(IdentityServiceApplicationContractsModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(SaasServiceApplicationContractsModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(ProductServiceApplicationContractsModule),
    typeof(ProductServiceEntityFrameworkCoreModule)
)]
public class MyProjectNameDbMigratorModule : AbpModule
{

}
