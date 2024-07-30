using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace MyCompanyName.MyProjectName.ProductService.Products;

[RemoteService(Name = ProductServiceRemoteServiceConsts.RemoteServiceName)]
[Area("productService")]
[Route("api/product-service/public/products")]
public class ProductPublicController : ProductServiceController, IProductPublicAppService
{
    private readonly IProductPublicAppService _productPublicAppService;

    public ProductPublicController(IProductPublicAppService productPublicAppService)
    {
        _productPublicAppService = productPublicAppService;
    }

    [HttpGet]
    public Task<List<ProductDto>> GetListAsync()
    {
        return _productPublicAppService.GetListAsync();
    }
}
