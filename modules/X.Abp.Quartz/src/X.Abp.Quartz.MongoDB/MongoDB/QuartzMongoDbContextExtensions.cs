using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Quartz.MongoDB;

public static class QuartzMongoDbContextExtensions
{
    public static void ConfigureQuartz(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
