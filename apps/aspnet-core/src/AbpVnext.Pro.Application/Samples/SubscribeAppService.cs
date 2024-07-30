// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using AbpVnext.Pro.Samples.Dtos;

using X.Abp.Notification;

namespace AbpVnext.Pro.Samples;

public class SubscribeAppService : ProAppService
{
    private readonly IUserNotificationSubscriptionManager _notificationSubscriptionManager;

    public SubscribeAppService(IUserNotificationSubscriptionManager notificationSubscriptionManager)
    {
        _notificationSubscriptionManager = notificationSubscriptionManager;
    }

    // 订阅一个一般通知
    public async Task SentFrendshipRequestAsync(UserIdentifier user)
    {
        await _notificationSubscriptionManager.SubscribeAsync(user, "SentFrendshipRequest");
    }

    // 订阅一个实体通知
    public async Task CommentPhotoAsync(Guid photoId, UserIdentifier user)
    {
        await _notificationSubscriptionManager.SubscribeAsync(user, "CommentPhoto", new EntityIdentifier(typeof(Photo), photoId));
    }
}
