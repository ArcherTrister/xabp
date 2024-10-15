using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.IdentityServer.MongoDB;

public static class IdentityServerProMongoDbContextExtensions
{
    public static void ConfigurePro(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
