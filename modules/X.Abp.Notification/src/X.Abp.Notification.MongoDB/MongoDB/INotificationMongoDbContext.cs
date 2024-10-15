using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Notification.MongoDB;

[ConnectionStringName(AbpNotificationDbProperties.ConnectionStringName)]
public interface INotificationMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
