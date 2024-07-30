using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Gdpr.MongoDB;

public static class GdprMongoDbContextExtensions
{
    public static void ConfigureGdpr(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
