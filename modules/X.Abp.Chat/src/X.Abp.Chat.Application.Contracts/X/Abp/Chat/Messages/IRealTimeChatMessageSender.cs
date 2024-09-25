// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

namespace X.Abp.Chat.Messages;

public interface IRealTimeChatMessageSender
{
    Task SendAsync(Guid targetUserId, ChatMessageRdto message);

    Task DeleteMessageAsync(Guid targetUserId, Guid messageId);

    Task DeleteConversationAsync(Guid targetUserId, Guid userId);
}
