// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Notification;

/// <summary>
/// Can be used to store a simple subject message as notification data.
/// </summary>
[Serializable]
public class SubjectMessageNotificationData : NotificationData
{
    private string _subject;

    /// <summary>
    /// The subject.
    /// </summary>
    public string Subject
    {
        get
        {
            return _subject ?? this[nameof(Subject)] as string;
        }

        set
        {
            this[nameof(Subject)] = value;
            _subject = value;
        }
    }

    private string _message;

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

    /// <summary>
    /// Needed for serialization.
    /// </summary>
    private SubjectMessageNotificationData()
    {
    }

    public SubjectMessageNotificationData(string subject, string message)
    {
        Subject = subject;
        Message = message;
    }
}
