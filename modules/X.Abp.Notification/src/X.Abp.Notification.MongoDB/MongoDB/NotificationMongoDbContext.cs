using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Notification.MongoDB;

[ConnectionStringName(AbpNotificationDbProperties.ConnectionStringName)]
public class NotificationMongoDbContext : AbpMongoDbContext, INotificationMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureNotification();
    }
}
