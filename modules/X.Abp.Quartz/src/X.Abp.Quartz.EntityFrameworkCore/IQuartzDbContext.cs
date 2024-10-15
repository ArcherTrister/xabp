using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Quartz;

[ConnectionStringName(AbpQuartzDbProperties.ConnectionStringName)]
public interface IQuartzDbContext : IEfCoreDbContext
{
}
