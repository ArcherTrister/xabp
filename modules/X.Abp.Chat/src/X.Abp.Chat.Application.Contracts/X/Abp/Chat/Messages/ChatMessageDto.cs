// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Chat.Messages;

public class ChatMessageDto
{
    public Guid Id { get; set; }

    public string Message { get; set; }

    public DateTime MessageDate { get; set; }

    public bool IsRead { get; set; }

    public DateTime ReadDate { get; set; }

    public ChatMessageSide Side { get; set; }
}
