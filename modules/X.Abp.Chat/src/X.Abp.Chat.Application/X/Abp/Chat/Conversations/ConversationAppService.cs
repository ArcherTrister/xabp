// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Features;
using Volo.Abp.Users;

using X.Abp.Chat.Features;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Permission;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.Conversations;

[RequiresFeature(AbpChatFeatures.Enable)]
[Authorize(AbpChatPermissions.Messaging)]
public class ConversationAppService : ChatAppServiceBase, IConversationAppService
{
    protected MessagingManager MessagingManager { get; }

    protected IChatUserLookupService ChatUserLookupService { get; }

    protected IConversationRepository ConversationRepository { get; }

    protected IRealTimeChatMessageSender RealTimeChatMessageSender { get; }

    public ConversationAppService(
        MessagingManager messagingManager,
        IChatUserLookupService chatUserLookupService,
        IConversationRepository conversationRepository,
        IRealTimeChatMessageSender realTimeChatMessageSender)
    {
        MessagingManager = messagingManager;
        ChatUserLookupService = chatUserLookupService;
        ConversationRepository = conversationRepository;
        RealTimeChatMessageSender = realTimeChatMessageSender;
    }

    public virtual async Task SendMessageAsync(SendMessageInput input)
    {
        var targetUser = await ChatUserLookupService.FindByIdAsync(input.TargetUserId);
        if (targetUser == null)
        {
            throw new BusinessException("X.Abp.Chat:010002");
        }

        var senderUser = await ChatUserLookupService.FindByIdAsync(CurrentUser.GetId());

        await MessagingManager.CreateNewMessage(
            CurrentUser.GetId(),
            targetUser.Id,
            input.Message);

        await RealTimeChatMessageSender.SendAsync(
            targetUser.Id,
            new ChatMessageRdto
            {
                SenderName = senderUser.Name,
                SenderSurname = senderUser.Surname,
                SenderUserId = senderUser.Id,
                SenderUsername = senderUser.UserName,
                Text = input.Message
            });
    }

    public virtual async Task<ChatConversationDto> GetConversationAsync(GetConversationInput input)
    {
        var targetUser = await ChatUserLookupService.FindByIdAsync(input.TargetUserId);
        if (targetUser == null)
        {
            throw new BusinessException("X.Abp.Chat:010003");
        }

        var chatConversation = new ChatConversationDto
        {
            TargetUserInfo = new ChatTargetUserInfo
            {
                UserId = targetUser.Id,
                Name = targetUser.Name,
                Surname = targetUser.Surname,
                Username = targetUser.UserName,
            },
            Messages = new List<ChatMessageDto>()
        };

        var messages = await MessagingManager.ReadMessagesAsync(targetUser.Id, input.SkipCount, input.MaxResultCount);

        chatConversation.Messages.AddRange(
            messages.Select(x => new ChatMessageDto
            {
                Message = x.Message.Text,
                MessageDate = x.Message.CreationTime,
                ReadDate = x.Message.ReadTime ?? DateTime.MaxValue,
                IsRead = x.Message.IsAllRead,
                Side = x.UserMessage.Side
            }));

        return chatConversation;
    }

    public virtual async Task MarkConversationAsReadAsync(MarkConversationAsReadInput input)
    {
        var conversationPair = await ConversationRepository.FindPairAsync(CurrentUser.GetId(), input.TargetUserId);

        if (conversationPair.SenderConversation.LastMessageSide == ChatMessageSide.Receiver)
        {
            conversationPair.SenderConversation.ResetUnreadMessageCount();
            await ConversationRepository.UpdateAsync(conversationPair.SenderConversation);
        }
    }
}
