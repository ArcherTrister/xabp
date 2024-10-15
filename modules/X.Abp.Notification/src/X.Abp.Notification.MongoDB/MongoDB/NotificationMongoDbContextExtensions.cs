using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Notification.MongoDB;

public static class NotificationMongoDbContextExtensions
{
    public static void ConfigureNotification(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
