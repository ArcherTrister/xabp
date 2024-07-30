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
    private readonly IPublisherAppService _publisherAppService;

    public PublisherAppService_Tests()
    {
        _realTimeNotifierManager = GetRequiredService<IRealTimeNotifierManager>();
        _publisherAppService = GetRequiredService<IPublisherAppService>();
    }

    [Fact]
    public async Task PublishAsync()
    {
        await _publisherAppService.PublishAsync(new Dtos.CreatePublishDto
        {
            Data = new NotificationData(),
            EntityIdentifier = null,
            ExcludedUserIds = null,
            NotificationName = "test",
            Severity = NotificationSeverity.Info,
            TargetNotifiers = new string[] { DefaultRealTimeNotifier.NotifierName },
            TenantIds = null,
            UserIds = new UserIdentifier[] { new UserIdentifier(null, System.Guid.NewGuid()) }
        });
    }

    [Fact]
    public Task Get_NotifiersAsync()
    {
        var notifiers = _realTimeNotifierManager.GetNotifiers();
        notifiers.Count.ShouldBe(1);
        return Task.CompletedTask;
    }
}
