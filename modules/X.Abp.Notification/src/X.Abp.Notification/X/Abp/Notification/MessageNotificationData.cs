// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Notification;

/// <summary>
/// Can be used to store a simple message as notification data.
/// </summary>
[Serializable]
public class MessageNotificationData : NotificationData
{
    /// <summary>
    /// The message.
    /// </summary>
    public string Message
    {
        get
        {
            return _message ?? this[nameof(Message)] as string;
        }

        set
        {
            this[nameof(Message)] = value;
            _message = value;
        }
    }

    private string _message;

    /// <summary>
    /// Needed for serialization.
    /// </summary>
    private MessageNotificationData()
    {
    }

    public MessageNotificationData(string message)
    {
        Message = message;
    }
}
