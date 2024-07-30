using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Guids;
using Xunit;

namespace MyCompanyName.MyProjectName.ProductService.Products;

public class ProductAppService_Tests : ProductServiceApplicationTestBase
{
    private readonly IProductAppService _productAppService;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ProductServiceTestData _testData;

    public ProductAppService_Tests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
        _testData = GetRequiredService<ProductServiceTestData>();
        _guidGenerator = GetRequiredService<IGuidGenerator>();
    }

    [Fact]
    public async Task Get_Products_Below_Price_Point()
    {
        var result = await _productAppService.GetListAsync(new GetProductsInput()
        {
            PriceMax = 2500
        });
        result.Items.ShouldContain(q => q.Name == _testData.BookName);
        result.Items.ShouldContain(q => q.Name == _testData.WatchName);
        result.TotalCount.ShouldBe(2);
    }

    [Fact]
    public async Task Get_Tv_Product()
    {
        var result = await _productAppService.GetAsync(_testData.TvId);
        result.ShouldNotBeNull();
        result.Name.ShouldBe(_testData.TvName);
    }

    [Fact]
    public async Task Update_Tv_Price()
    {
        var tv = await _productAppService.GetAsync(_testData.TvId);
        var tvUpdateDto = new ProductUpdateDto
        {
            Name = tv.Name,
            Price = tv.Price * 0.9f //10% discount
        };

        var updatedTv = await _productAppService.UpdateAsync(_testData.TvId, tvUpdateDto);
        updatedTv.Price.ShouldBeLessThan(_testData.TvPrice);
    }

    [Fact]
    public async Task Create_New_Laptop_Product()
    {
        var laptop = new ProductCreateDto
        {
            Name = "Dell 15.6\" i7 laptop",
            Price = 8994.99f
        };
        var createdProduct = await _productAppService.CreateAsync(laptop);
        createdProduct.Id.ShouldNotBe(Guid.Empty);
    }
    [Fact]
    public async Task Delete_Watch_Product()
    {
        await _productAppService.DeleteAsync(_testData.WatchId);
        var products = await _productAppService.GetListAsync(new GetProductsInput());
        products.TotalCount.ShouldBe(2);
    }
}
