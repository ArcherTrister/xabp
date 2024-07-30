using System;
using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.ProductService;

public class ProductServiceTestData : ISingletonDependency
{
    public Guid TvId { get; } = Guid.NewGuid();

    public string TvName { get; } = "Super New 105\" TV ";

    public float TvPrice { get; } = 45900.90f;

    public Guid BookId { get; } = Guid.NewGuid();

    public string BookName { get; } = "The Hitchhiker's Guide to the Galaxy";

    public float BookPrice { get; } = 12.50f;

    public Guid WatchId { get; } = Guid.NewGuid();

    public string WatchName { get; } = "Swiss Diamond Watch for Men";

    public float WatchPrice { get; } = 249.50f;
}
