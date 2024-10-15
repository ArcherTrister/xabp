using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.OpenIddict.MongoDB;

public static class OpenIddictProMongoDbContextExtensions
{
    public static void ConfigureOpenIddictPro(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
