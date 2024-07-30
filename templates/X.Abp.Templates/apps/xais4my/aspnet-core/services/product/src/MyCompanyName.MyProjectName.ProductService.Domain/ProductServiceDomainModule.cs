using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.ProductService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ProductServiceDomainSharedModule)
)]
public class ProductServiceDomainModule : AbpModule
{

}
