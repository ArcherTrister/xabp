// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.Conversations;

public class ChatConversationDto
{
    public List<ChatMessageDto> Messages { get; set; }

    public ChatTargetUserInfo TargetUserInfo { get; set; }
}
