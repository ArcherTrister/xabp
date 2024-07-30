// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using X.Abp.Notification;

namespace AbpVnext.Pro.Samples.Dtos;

[Serializable]
public class SentFrendshipRequestNotificationData : NotificationData
{
    public string SenderUserName { get; set; }

    public string FriendshipMessage { get; set; }

    public SentFrendshipRequestNotificationData(string senderUserName, string friendshipMessage)
    {
        SenderUserName = senderUserName;
        FriendshipMessage = friendshipMessage;
    }
}
