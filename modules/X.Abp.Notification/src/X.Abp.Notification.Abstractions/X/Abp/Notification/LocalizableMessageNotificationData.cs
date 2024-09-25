// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Localization;

namespace X.Abp.Notification;

/// <summary>
/// Can be used to store a simple message as notification data.
/// </summary>
[Serializable]
public class LocalizableMessageNotificationData : NotificationData
{
    /// <summary>
    /// The message.
    /// </summary>
    public LocalizableString Message
    {
        get
        {
            return _message ?? this[nameof(Message)] as LocalizableString;
        }

        set
        {
            this[nameof(Message)] = value;
            _message = value;
        }
    }

    private LocalizableString _message;

    /// <summary>
    /// Needed for serialization.
    /// </summary>
    private LocalizableMessageNotificationData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizableMessageNotificationData"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public LocalizableMessageNotificationData(LocalizableString message)
    {
        Message = message;
    }
}
