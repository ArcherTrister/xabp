// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace X.Abp.Chat.Messages;

[Dependency(TryRegister = true)]
public class DistributedEventBusRealTimeChatMessageSender : IRealTimeChatMessageSender, ITransientDependency
{
    protected IDistributedEventBus DistributedEventBus { get; }

    public DistributedEventBusRealTimeChatMessageSender(IDistributedEventBus distributedEventBus)
    {
        DistributedEventBus = distributedEventBus;
    }

    public virtual async Task SendAsync(Guid targetUserId, ChatMessageRdto message)
    {
        await DistributedEventBus.PublishAsync(
            new ChatMessageEto
            {
                SenderUserId = message.SenderUserId,
                SenderUserName = message.SenderUsername,
                SenderName = message.SenderName,
                SenderSurname = message.SenderSurname,
                TargetUserId = targetUserId,
                Message = message.Text
            });
    }
}
