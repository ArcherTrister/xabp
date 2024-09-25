// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Features;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
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

    protected IPermissionFinder PermissionFinder { get; }

    public ConversationAppService(
        MessagingManager messagingManager,
        IChatUserLookupService chatUserLookupService,
        IConversationRepository conversationRepository,
        IRealTimeChatMessageSender realTimeChatMessageSender,
        IPermissionFinder permissionFinder)
    {
        MessagingManager = messagingManager;
        ChatUserLookupService = chatUserLookupService;
        ConversationRepository = conversationRepository;
        RealTimeChatMessageSender = realTimeChatMessageSender;
        PermissionFinder = permissionFinder;
    }

    public virtual async Task<ChatMessageDto> SendMessageAsync(SendMessageInput input)
    {
        ChatUser targetUser = await ChatUserLookupService.FindByIdAsync(input.TargetUserId);
        if (targetUser == null)
        {
            throw new BusinessException("X.Abp.Chat:010002");
        }

        if (!await PermissionFinder.IsGrantedAsync(targetUser.Id, AbpChatPermissions.Messaging))
        {
            throw new BusinessException("X.Abp.Chat:010004");
        }

        if (!await AuthorizationService.IsGrantedAsync(AbpChatPermissions.Searching))
        {
            if (!await MessagingManager.HasConversationAsync(targetUser.Id))
            {
                throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenRequirementHasNotGrantedForGivenResource);
            }
        }

        Message message;
        using (IUnitOfWork uow = UnitOfWorkManager.Begin(true))
        {
            message = await MessagingManager.CreateNewMessage(CurrentUser.GetId(), targetUser.Id, input.Message);
            await uow.CompleteAsync();
        }

        ChatUser chatUser = await ChatUserLookupService.FindByIdAsync(CurrentUser.GetId());
        await RealTimeChatMessageSender.SendAsync(targetUser.Id, new ChatMessageRdto()
        {
            Id = message.Id,
            SenderName = chatUser.Name,
            SenderSurname = chatUser.Surname,
            SenderUserId = chatUser.Id,
            SenderUsername = chatUser.UserName,
            Text = input.Message
        });
        ChatMessageDto chatMessageDto = new ChatMessageDto()
        {
            Id = message.Id,
            Message = message.Text,
            MessageDate = message.CreationTime,
            ReadDate = message.ReadTime ?? DateTime.MaxValue,
            IsRead = message.IsAllRead,
            Side = ChatMessageSide.Sender
        };

        return chatMessageDto;
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
                Id = x.Message.Id,
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
        try
        {
            using (IUnitOfWork uow = UnitOfWorkManager.Begin(true, UnitOfWorkManager.Current != null && UnitOfWorkManager.Current.Options.IsTransactional))
            {
                ConversationPair conversationPair = await ConversationRepository.FindPairAsync(CurrentUser.GetId(), input.TargetUserId);
                if (conversationPair.SenderConversation.LastMessageSide == ChatMessageSide.Receiver)
                {
                    conversationPair.SenderConversation.ResetUnreadMessageCount();
                    Conversation conversation = await ConversationRepository.UpdateAsync(conversationPair.SenderConversation);
                }

                await uow.CompleteAsync();
            }
        }
        catch (AbpDbConcurrencyException ex)
        {
        }
    }

    public virtual async Task DeleteMessageAsync(DeleteMessageInput input)
    {
        await MessagingManager.DeleteMessage(input.MessageId, CurrentUser.GetId(), input.TargetUserId);

        await RealTimeChatMessageSender.DeleteMessageAsync(input.TargetUserId, input.MessageId);
    }

    public virtual async Task DeleteConversationAsync(DeleteConversationInput input)
    {
        await MessagingManager.DeleteConversationAsync(CurrentUser.GetId(), input.TargetUserId);

        await RealTimeChatMessageSender.DeleteConversationAsync(input.TargetUserId, CurrentUser.GetId());
    }
}
