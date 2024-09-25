using MyCompanyName.MyProjectName.ProductService.Localization;
using Volo.Abp.Application.Services;

namespace MyCompanyName.MyProjectName.ProductService;

public abstract class ProductServiceAppService : ApplicationService
{
    protected ProductServiceAppService()
    {
        LocalizationResource = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceApplicationModule);
    }
}
