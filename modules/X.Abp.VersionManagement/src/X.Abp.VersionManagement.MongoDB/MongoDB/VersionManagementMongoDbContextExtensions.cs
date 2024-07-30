using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.VersionManagement.MongoDB;

public static class VersionManagementMongoDbContextExtensions
{
    public static void ConfigureVersionManagement(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
