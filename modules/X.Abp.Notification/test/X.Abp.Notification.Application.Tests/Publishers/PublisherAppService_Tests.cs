// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Shouldly;

using X.Abp.Notification.RealTime;

using Xunit;

namespace X.Abp.Notification.Publishers;

public class PublisherAppService_Tests : NotificationApplicationTestBase
{
    private readonly IRealTimeNotifierManager _realTimeNotifierManager;
    private readonly INotificationPublisher _notificationPublisher;

    public PublisherAppService_Tests()
    {
        _realTimeNotifierManager = GetRequiredService<IRealTimeNotifierManager>();
        _notificationPublisher = GetRequiredService<INotificationPublisher>();
    }

    [Fact]
    public virtual async Task Publish_Test()
    {
        await _notificationPublisher.PublishAsync("test", new NotificationData(), null, NotificationSeverity.Info, new UserIdentifier[] { new UserIdentifier(null, System.Guid.NewGuid()) }, null, null, new string[] { NullRealTimeNotifier.NotifierName });
    }

    [Fact]
    public virtual Task Notifiers_Test()
    {
        var notifiers = _realTimeNotifierManager.Notifiers;
        notifiers.Count.ShouldBe(1);
        return Task.CompletedTask;
    }
}
