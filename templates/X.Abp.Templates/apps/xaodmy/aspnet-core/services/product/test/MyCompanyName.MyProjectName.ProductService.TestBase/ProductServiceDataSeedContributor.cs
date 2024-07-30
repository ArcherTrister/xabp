using System.Threading.Tasks;
using MyCompanyName.MyProjectName.ProductService.Products;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.ProductService;

public class ProductServiceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ProductServiceTestData _testData;
    private readonly IProductRepository _productRepository;

    public ProductServiceDataSeedContributor(
        ProductServiceTestData testData,
        IProductRepository productRepository)
    {
        _testData = testData;
        _productRepository = productRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedProductsAsync();
    }

    private async Task SeedProductsAsync()
    {
        var tv = new Product(_testData.TvId, _testData.TvName, _testData.TvPrice);
        await _productRepository.InsertAsync(tv);

        var book = new Product(_testData.BookId, _testData.BookName, _testData.BookPrice);
        await _productRepository.InsertAsync(book);

        var watch = new Product(_testData.WatchId, _testData.WatchName, _testData.WatchPrice);
        await _productRepository.InsertAsync(watch);
    }
}
