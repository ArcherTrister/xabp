// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Users;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Settings;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.Messages;

public class MessagingManager : DomainService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserMessageRepository _userMessageRepository;
    private readonly IChatUserLookupService _chatUserLookupService;
    private readonly IConversationRepository _conversationRepository;

    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected ISettingProvider SettingProvider { get; }

    protected ICurrentUser CurrentUser { get; }

    public MessagingManager(
        IMessageRepository messageRepository,
        IUserMessageRepository userMessageRepository,
        IChatUserLookupService chatUserLookupService,
        IConversationRepository conversationRepository,
        UnitOfWorkManager unitOfWorkManager,
        ISettingProvider settingProvider,
        ICurrentUser currentUser)
    {
        _messageRepository = messageRepository;
        _userMessageRepository = userMessageRepository;
        _chatUserLookupService = chatUserLookupService;
        _conversationRepository = conversationRepository;
        UnitOfWorkManager = unitOfWorkManager;
        SettingProvider = settingProvider;
        CurrentUser = currentUser;
    }

    public virtual async Task<Message> CreateNewMessage(Guid senderId, Guid receiverId, string messageText)
    {
        Check.NotNullOrWhiteSpace(messageText, nameof(messageText));
        if (await _chatUserLookupService.FindByIdAsync(receiverId) == null)
        {
            throw new BusinessException("X.Abp.Chat:010002");
        }

        Message message = await _messageRepository.InsertAsync(new Message(GuidGenerator.Create(), messageText, CurrentTenant.Id));
        await _userMessageRepository.InsertAsync(new UserMessage(GuidGenerator.Create(), senderId, message.Id, ChatMessageSide.Sender, receiverId, CurrentTenant.Id));
        await _userMessageRepository.InsertAsync(new UserMessage(GuidGenerator.Create(), receiverId, message.Id, ChatMessageSide.Receiver, senderId, CurrentTenant.Id));
        await CreateOrUpdateConversationWithNewMessageAsync(senderId, receiverId, messageText);

        return message;
    }

    public virtual async Task DeleteMessage(Guid messageId, Guid senderId, Guid targetUserId)
    {
        List<UserMessage> userMessages = await _userMessageRepository.GetListAsync(messageId);

        if (!userMessages.All(message => message.UserId != senderId))
        {
            var message = await _messageRepository.GetAsync(messageId);
            if (message != null)
            {
                await CheckDeletingMessageSetting(message);
                await _userMessageRepository.DeleteManyAsync(userMessages);
                await _messageRepository.DeleteAsync(message);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                await UpdateConversationLastMessageAsync(senderId, targetUserId, message.Text);
            }
        }
    }

    public virtual async Task DeleteConversationAsync(Guid senderId, Guid targetId)
    {
        await CheckDeletingConversationSetting();

        ConversationPair conversationPair = await _conversationRepository.FindPairAsync(senderId, targetId);
        if (conversationPair != null)
        {
             await _conversationRepository.DeleteManyAsync(new Conversation[2]
            {
          conversationPair.SenderConversation,
          conversationPair.TargetConversation
            });
        }

        List<Guid> messageIds = await _userMessageRepository.GetAllMessageIdsAsync(senderId, targetId);
        if (messageIds.Any())
        {
            await _messageRepository.DeleteALlMessagesAsync(messageIds);
        }

        await _userMessageRepository.DeleteAllMessages(senderId, targetId);
    }

    public virtual async Task<List<MessageWithDetails>> ReadMessagesAsync(Guid targetUserId, int skipCount, int maxResultCount)
    {
        ConversationPair conversationPair = await _conversationRepository.FindPairAsync(CurrentUser.GetId(), targetUserId);
        if (conversationPair != null)
        {
            conversationPair.SenderConversation?.ResetUnreadMessageCount();
            if (conversationPair.SenderConversation != null)
            {
                await _conversationRepository.UpdateAsync(conversationPair.SenderConversation);
            }

            if (conversationPair.TargetConversation != null)
            {
                await _conversationRepository.UpdateAsync(conversationPair.TargetConversation);
            }
        }

        List<MessageWithDetails> messages = new List<MessageWithDetails>();
        try
        {
            using (IUnitOfWork uow = UnitOfWorkManager.Begin(true, UnitOfWorkManager.Current != null && UnitOfWorkManager.Current.Options.IsTransactional))
            {
                messages = await _userMessageRepository.GetMessagesAsync(CurrentUser.GetId(), targetUserId, skipCount, maxResultCount);
                List<Message> readMessages = new List<Message>();
                MessageWithDetails[] messageWithDetailsArray = messages.Where(m => !m.UserMessage.IsRead).ToArray();
                for (int index = 0; index < messageWithDetailsArray.Length; ++index)
                {
                    MessageWithDetails message = messageWithDetailsArray[index];
                    message.UserMessage.MarkAsRead(Clock.Now);
                    await _userMessageRepository.UpdateAsync(message.UserMessage);
                    message.Message.MarkAsAllRead(Clock.Now);
                    readMessages.Add(message.Message);
                }

                messageWithDetailsArray = null;
                foreach (Message entity in readMessages)
                {
                    await _messageRepository.UpdateAsync(entity);
                }

                await uow.CompleteAsync();
            }
        }
        catch (AbpDbConcurrencyException ex)
        {
        }

        return messages;
    }

    public virtual Task<bool> HasConversationAsync(Guid targetUserId)
    {
        return _userMessageRepository.HasConversationAsync(CurrentUser.GetId(), targetUserId);
    }

    private async Task CreateOrUpdateConversationWithNewMessageAsync(Guid senderId, Guid receiverId, string messageText)
    {
        DateTime now = Clock.Now;

        ConversationPair conversationPair = await _conversationRepository.FindPairAsync(senderId, receiverId) ?? new ConversationPair();

        if (conversationPair.SenderConversation == null)
        {
            var senderConversation = new Conversation(GuidGenerator.Create(), senderId, receiverId, CurrentTenant.Id)
            {
                LastMessageSide = ChatMessageSide.Sender,
                LastMessage = messageText,
                LastMessageDate = now
            };
            await _conversationRepository.InsertAsync(senderConversation);

            conversationPair.SenderConversation = senderConversation;
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        if (conversationPair.TargetConversation == null)
        {
            var targetConversation = new Conversation(GuidGenerator.Create(), receiverId, senderId, CurrentTenant.Id)
            {
                LastMessageSide = ChatMessageSide.Receiver,
                LastMessage = messageText,
                LastMessageDate = now
            };
            await _conversationRepository.InsertAsync(targetConversation);

            conversationPair.TargetConversation = targetConversation;
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        conversationPair.SenderConversation.SetLastMessage(messageText, now, ChatMessageSide.Sender);
        conversationPair.TargetConversation.SetLastMessage(messageText, now, ChatMessageSide.Receiver);

        await _conversationRepository.UpdateAsync(conversationPair.SenderConversation);
        await _conversationRepository.UpdateAsync(conversationPair.TargetConversation);
    }

    private async Task UpdateConversationLastMessageAsync(Guid senderId, Guid receiverId, string deletedText)
    {
        ConversationPair conversationPair = await _conversationRepository.FindPairAsync(senderId, receiverId);
        MessageWithDetails messageWithDetails = await _userMessageRepository.GetLastMessageAsync(senderId, receiverId);
        DateTime messageTime = Clock.Now;
        string messageText = string.Empty;

        if (messageWithDetails != null)
        {
            messageTime = messageWithDetails.Message.CreationTime;
            messageText = messageWithDetails.Message.Text;
        }

        if (conversationPair?.SenderConversation != null && conversationPair.SenderConversation.LastMessage == deletedText)
        {
            conversationPair.SenderConversation.SetLastMessage(messageText, messageTime, ChatMessageSide.Sender, true);
            await _conversationRepository.UpdateAsync(conversationPair.SenderConversation);
        }

        if (conversationPair?.TargetConversation != null && conversationPair.TargetConversation.LastMessage == deletedText)
        {
            conversationPair.TargetConversation.SetLastMessage(messageText, messageTime, ChatMessageSide.Sender, true);
            await _conversationRepository.UpdateAsync(conversationPair.TargetConversation);
        }
    }

    protected virtual async Task CheckDeletingMessageSetting(Message message)
    {
        ChatDeletingMessages chatDeletingMessages = (ChatDeletingMessages)Enum.Parse(typeof(ChatDeletingMessages), await SettingProvider.GetOrNullAsync(AbpChatSettings.Messaging.DeletingMessages));
        if (chatDeletingMessages == ChatDeletingMessages.Disabled)
        {
            throw new BusinessException("X.Abp.Chat:010007");
        }

        if (chatDeletingMessages != ChatDeletingMessages.EnabledWithDeletionPeriod)
        {
            return;
        }

        int messageDeletionPeriod = await SettingProvider.GetAsync<int>("X.Abp.Chat.Messaging.MessageDeletionPeriod");
        if (message.CreationTime.AddSeconds(messageDeletionPeriod) < Clock.Now)
        {
            throw new BusinessException("X.Abp.Chat:010005").WithData("seconds", messageDeletionPeriod);
        }
    }

    protected virtual async Task CheckDeletingConversationSetting()
    {
        if ((ChatDeletingMessages)Enum.Parse(typeof(ChatDeletingMessages), await SettingProvider.GetOrNullAsync(AbpChatSettings.Messaging.DeletingMessages)) != ChatDeletingMessages.Enabled)
        {
            throw new BusinessException("X.Abp.Chat:010006");
        }

        if ((ChatDeletingConversations)Enum.Parse(typeof(ChatDeletingConversations), await SettingProvider.GetOrNullAsync(AbpChatSettings.Messaging.DeletingConversations)) == ChatDeletingConversations.Disabled)
        {
            throw new BusinessException("X.Abp.Chat:010006");
        }
    }
}
