using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.CmsKit.Pro.MongoDB;

public static class ProMongoDbContextExtensions
{
    public static void ConfigurePro(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
