using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace X.Abp.Quartz;

public static class QuartzDbContextModelBuilderExtensions
{
    public static void ConfigureQuartz(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.TryConfigureObjectExtensions<QuartzDbContext>();
    }
}
