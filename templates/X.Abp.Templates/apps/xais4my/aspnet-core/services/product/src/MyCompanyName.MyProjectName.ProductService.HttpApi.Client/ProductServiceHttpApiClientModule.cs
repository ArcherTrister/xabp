using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace MyCompanyName.MyProjectName.ProductService;

[DependsOn(
    typeof(ProductServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ProductServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(ProductServiceApplicationContractsModule).Assembly,
            ProductServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProductServiceHttpApiClientModule>();
        });
    }
}
