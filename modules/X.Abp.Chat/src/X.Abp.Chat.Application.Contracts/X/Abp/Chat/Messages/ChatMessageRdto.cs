// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Chat.Messages;
public class ChatMessageRdto
{
    public Guid SenderUserId { get; set; }

    public Guid Id { get; set; }

    public string SenderUsername { get; set; }

    public string SenderName { get; set; }

    public string SenderSurname { get; set; }

    public string Text { get; set; }
}
