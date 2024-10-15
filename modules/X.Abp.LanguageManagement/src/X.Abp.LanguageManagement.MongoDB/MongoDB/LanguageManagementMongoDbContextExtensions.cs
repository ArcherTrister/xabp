using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.LanguageManagement;

public static class LanguageManagementMongoDbContextExtensions
{
    public static void ConfigureLanguageManagement(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
