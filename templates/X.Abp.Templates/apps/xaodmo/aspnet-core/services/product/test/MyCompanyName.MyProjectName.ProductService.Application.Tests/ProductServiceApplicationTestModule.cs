using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.ProductService;

[DependsOn(
    typeof(ProductServiceApplicationModule),
    typeof(ProductServiceDomainTestModule)
    )]
public class ProductServiceApplicationTestModule : AbpModule
{

}
