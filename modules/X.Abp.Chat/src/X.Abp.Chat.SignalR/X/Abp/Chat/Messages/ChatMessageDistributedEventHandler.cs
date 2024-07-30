// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace X.Abp.Chat.Messages;

public class ChatMessageDistributedEventHandler : IDistributedEventHandler<ChatMessageEto>, ITransientDependency
{
    private readonly IRealTimeChatMessageSender _realTimeChatMessageSender;

    public ChatMessageDistributedEventHandler(IRealTimeChatMessageSender realTimeChatMessageSender)
    {
        _realTimeChatMessageSender = realTimeChatMessageSender;
    }

    public async Task HandleEventAsync(ChatMessageEto eventData)
    {
        await _realTimeChatMessageSender.SendAsync(
            eventData.TargetUserId,
            new ChatMessageRdto
            {
                SenderUserId = eventData.SenderUserId,
                SenderUsername = eventData.SenderUserName,
                SenderName = eventData.SenderName,
                SenderSurname = eventData.SenderSurname,
                Text = eventData.Message
            });
    }
}
