// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.Conversations;

public class Conversation : Entity<Guid>, IMultiTenant, IAggregateRoot<Guid>
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid UserId { get; protected set; }

    public virtual Guid TargetUserId { get; protected set; }

    public virtual ChatMessageSide LastMessageSide { get; set; }

    public virtual string LastMessage { get; set; }

    public virtual DateTime LastMessageDate { get; set; }

    public virtual int UnreadMessageCount { get; protected set; }

    protected Conversation()
    {
    }

    public Conversation(Guid id, Guid userId, Guid targetUserId, Guid? tenantId = null)
    : base(id)
    {
        UserId = userId;
        TargetUserId = targetUserId;
        TenantId = tenantId;
    }

    public virtual void AddUnreadMessage(int count = 1)
    {
        UnreadMessageCount += count;
    }

    public virtual void ResetUnreadMessageCount()
    {
        UnreadMessageCount = 0;
    }

    public void SetLastMessage(string messageText, DateTime messageTime, ChatMessageSide messageSide)
    {
        LastMessage = Check.NotNullOrWhiteSpace(messageText, nameof(messageText));
        LastMessageDate = messageTime;
        LastMessageSide = messageSide;

        if (messageSide == ChatMessageSide.Sender)
        {
            ResetUnreadMessageCount();
        }
        else
        {
            AddUnreadMessage();
        }
    }
}
