using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyCompanyName.MyProjectName.ProductService.Products;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<List<Product>> GetListAsync(
        string filterText = null,
        string name = null,
        float? priceMin = null,
        float? priceMax = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        string filterText = null,
        string name = null,
        float? priceMin = null,
        float? priceMax = null,
        CancellationToken cancellationToken = default
    );
}
