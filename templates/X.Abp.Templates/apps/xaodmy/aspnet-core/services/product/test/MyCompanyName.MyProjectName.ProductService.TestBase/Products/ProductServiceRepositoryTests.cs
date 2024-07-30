using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace MyCompanyName.MyProjectName.ProductService.Products;

public abstract class ProductServiceRepositoryTests<TStartupModule>
    : ProductServiceTestBase<TStartupModule>
        where TStartupModule : IAbpModule
{
    private readonly IProductRepository _productRepository;
    private readonly ProductServiceTestData _testData;

    protected ProductServiceRepositoryTests()
    {
        _productRepository = GetRequiredService<IProductRepository>();
        _testData = GetRequiredService<ProductServiceTestData>();
    }

    [Fact]
    public async Task Get_All_Product_List()
    {
        var result = await _productRepository.GetListAsync();
        result.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Get_All_Product_List_Filtered_By_Name()
    {
        var watch = await _productRepository.GetListAsync(filterText: "Swiss Diamond");
        watch.Count.ShouldBe(1);
        watch.ShouldContain(q => q.Id == _testData.WatchId);

        var book = await _productRepository.GetListAsync(name: "The Hitchhiker's");
        watch.Count.ShouldBe(1);
        watch.ShouldContain(q => q.Id == _testData.WatchId);
    }

    [Fact]
    public async Task Get_Product_List_Filtered_By_Price()
    {
        var under10k_products = await _productRepository.GetListAsync(priceMax: 10000f);
        under10k_products.ShouldContain(q => q.Name == _testData.BookName);
        under10k_products.ShouldContain(q => q.Name == _testData.WatchName);

        var over10k_products = await _productRepository.GetListAsync(priceMin: 10000f);
        over10k_products.ShouldContain(q => q.Name == _testData.TvName);
    }

    [Fact]
    public async Task Get_Product_Count_Filtered_By_Price()
    {
        var under10k_productsCount = await _productRepository.GetCountAsync(priceMax: 10000f);
        under10k_productsCount.ShouldBe(2);

        var over10k_productsCount = await _productRepository.GetCountAsync(priceMin: 10000f);
        over10k_productsCount.ShouldBe(1);
    }
}
