// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Chat.Messages;

[Dependency(ReplaceServices = true)]
public class SignalRRealTimeChatMessageSender : IRealTimeChatMessageSender, ITransientDependency
{
    protected IHubContext<ChatHub> ChatHubContext { get; }

    public SignalRRealTimeChatMessageSender(IHubContext<ChatHub> chatHubContext)
    {
        ChatHubContext = chatHubContext;
    }

    public virtual async Task SendAsync(Guid targetUserId, ChatMessageRdto message)
    {
        await ChatHubContext
            .Clients
            .User(targetUserId.ToString())
            .SendAsync("ReceiveMessage", message);
    }

    public virtual async Task DeleteMessageAsync(Guid targetUserId, Guid messageId)
    {
        await ChatHubContext.Clients.User(targetUserId.ToString()).SendAsync("DeleteMessage", messageId);
    }

    public virtual async Task DeleteConversationAsync(Guid targetUserId, Guid userId)
    {
        await ChatHubContext.Clients.User(targetUserId.ToString()).SendAsync("DeleteConversation", userId);
    }
}
