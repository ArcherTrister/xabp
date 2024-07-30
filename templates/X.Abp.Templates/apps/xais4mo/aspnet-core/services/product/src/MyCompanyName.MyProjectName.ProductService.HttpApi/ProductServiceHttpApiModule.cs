using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using MyCompanyName.MyProjectName.ProductService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.ProductService;

[DependsOn(
    typeof(ProductServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class ProductServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ProductServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ProductServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
