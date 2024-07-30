using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Forms.MongoDB;

public static class FormsMongoDbContextExtensions
{
    public static void ConfigureForms(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
