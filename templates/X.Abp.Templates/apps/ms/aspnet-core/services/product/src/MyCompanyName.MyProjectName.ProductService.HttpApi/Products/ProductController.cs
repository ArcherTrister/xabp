using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace MyCompanyName.MyProjectName.ProductService.Products;

[RemoteService(Name = ProductServiceRemoteServiceConsts.RemoteServiceName)]
[Area("productService")]
[Route("api/product-service/products")]
public class ProductController : ProductServiceController, IProductAppService
{
    private readonly IProductAppService _productAppService;

    public ProductController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ProductDto>> GetListAsync(GetProductsInput input)
    {
        return _productAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ProductDto> GetAsync(Guid id)
    {
        return _productAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<ProductDto> CreateAsync(ProductCreateDto input)
    {
        return _productAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
    {
        return _productAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _productAppService.DeleteAsync(id);
    }
}
