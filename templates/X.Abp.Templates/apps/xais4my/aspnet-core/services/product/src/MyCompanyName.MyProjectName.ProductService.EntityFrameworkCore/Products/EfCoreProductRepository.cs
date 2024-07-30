using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.MyProjectName.ProductService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.ProductService.Products;

public class EfCoreProductRepository : EfCoreRepository<ProductServiceDbContext, Product, Guid>, IProductRepository
{
    public EfCoreProductRepository(IDbContextProvider<ProductServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    public async Task<List<Product>> GetListAsync(
        string filterText = null,
        string name = null,
        float? priceMin = null,
        float? priceMax = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(await GetDbSetAsync(), filterText, name, priceMin, priceMax);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProductConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(
        string filterText = null,
        string name = null,
        float? priceMin = null,
        float? priceMax = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(await GetDbSetAsync(), filterText, name, priceMin, priceMax);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Product> ApplyFilter(
        IQueryable<Product> query,
        string filterText,
        string name = null,
        float? priceMin = null,
        float? priceMax = null)
    {
        return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(priceMin.HasValue, e => e.Price >= priceMin.Value)
                .WhereIf(priceMax.HasValue, e => e.Price <= priceMax.Value);
    }
}
