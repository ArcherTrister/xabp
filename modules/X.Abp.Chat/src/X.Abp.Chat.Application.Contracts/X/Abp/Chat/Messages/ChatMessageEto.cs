// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Chat.Messages;

public class ChatMessageEto
{
    public Guid TargetUserId { get; set; }

    public string SenderUserName { get; set; }

    public string SenderName { get; set; }

    public string SenderSurname { get; set; }

    public Guid SenderUserId { get; set; }

    public string Message { get; set; }

    public Guid MessageId { get; set; }
}
