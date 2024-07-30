using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Saas.MongoDB;

public static class SaasMongoDbContextExtensions
{
    public static void ConfigureSaas(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
