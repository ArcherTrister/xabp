using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MyCompanyName.MyProjectName.ProductService.Permissions;
using Volo.Abp.Application.Dtos;

namespace MyCompanyName.MyProjectName.ProductService.Products;

[Authorize(ProductServicePermissions.Products.Default)]
public class ProductAppService : ProductServiceAppService, IProductAppService
{
    private readonly IProductRepository _productRepository;

    public ProductAppService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public virtual async Task<PagedResultDto<ProductDto>> GetListAsync(GetProductsInput input)
    {
        var totalCount = await _productRepository.GetCountAsync(input.FilterText, input.Name, input.PriceMin, input.PriceMax);
        var items = await _productRepository.GetListAsync(input.FilterText, input.Name, input.PriceMin, input.PriceMax, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<ProductDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Product>, List<ProductDto>>(items)
        };
    }

    public virtual async Task<ProductDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Product, ProductDto>(await _productRepository.GetAsync(id));
    }

    [Authorize(ProductServicePermissions.Products.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }

    [Authorize(ProductServicePermissions.Products.Create)]
    public virtual async Task<ProductDto> CreateAsync(ProductCreateDto input)
    {
        var product = new Product(GuidGenerator.Create(), input.Name, input.Price, CurrentTenant.Id);

        await _productRepository.InsertAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    [Authorize(ProductServicePermissions.Products.Edit)]
    public virtual async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
    {
        var product = await _productRepository.GetAsync(id);

        product.SetName(input.Name);
        product.Price = input.Price;

        await _productRepository.UpdateAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}
