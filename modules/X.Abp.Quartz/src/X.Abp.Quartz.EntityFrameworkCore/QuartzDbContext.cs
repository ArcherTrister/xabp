using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Quartz;

[ConnectionStringName(AbpQuartzDbProperties.ConnectionStringName)]
public class QuartzDbContext : AbpDbContext<QuartzDbContext>, IQuartzDbContext
{
    public QuartzDbContext(DbContextOptions<QuartzDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureQuartz();
    }
}
