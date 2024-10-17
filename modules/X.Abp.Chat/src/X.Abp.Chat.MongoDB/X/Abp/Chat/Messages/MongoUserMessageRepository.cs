// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.MongoDB.Messages
{
    public class MongoUserMessageRepository : MongoDbRepository<IChatMongoDbContext, UserMessage, Guid>, IUserMessageRepository
    {
        public MongoUserMessageRepository(IMongoDbContextProvider<IChatMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<List<MessageWithDetails>> GetMessagesAsync(
          Guid userId,
          Guid targetUserId,
          int skipCount,
          int maxResultCount,
          CancellationToken cancellationToken = default)
        {
            IMongoQueryable<UserMessage> userMessageQueryable = await GetMongoQueryableAsync(cancellationToken, null);
            IChatMongoDbContext chatMongoDbContext = await GetDbContextAsync(cancellationToken);

            IMongoQueryable<MessageWithDetails> messageListQuery = ApplyDataFilters<IMongoQueryable<Message>, Message>(userMessageQueryable.Join(chatMongoDbContext.ChatMessages, chatUserMessage => chatUserMessage.ChatMessageId, message => message.Id, (chatUserMessage, message) => new
            {
                chatUserMessage = chatUserMessage,
                message = message
            })
                .Where(data => userId == data.chatUserMessage.UserId && targetUserId == data.chatUserMessage.TargetUserId)
                .Select(data => data.message))
                .Select(p => new MessageWithDetails { Message = p });

            List<MessageWithDetails> chatMessageWithDetailsList = await messageListQuery.OrderByDescending(x => x.Message.CreationTime)
                .PageBy<MessageWithDetails, IMongoQueryable<MessageWithDetails>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            List<UserMessage> userMessageList = await userMessageQueryable.Where(x => x.UserId == userId && x.TargetUserId == targetUserId)
                .PageBy<UserMessage, IMongoQueryable<UserMessage>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            foreach (MessageWithDetails messageWithDetails in chatMessageWithDetailsList)
            {
                messageWithDetails.UserMessage = userMessageList.Find(x => x.ChatMessageId == messageWithDetails.Message.Id);
            }

            return chatMessageWithDetailsList;
        }

        public async Task<MessageWithDetails> GetLastMessageAsync(
          Guid userId,
          Guid targetUserId,
          CancellationToken cancellationToken = default)
        {
            IMongoQueryable<UserMessage> userMessageQueryable = await GetMongoQueryableAsync(cancellationToken, null);
            IChatMongoDbContext chatMongoDbContext = await GetDbContextAsync(cancellationToken);

            IMongoQueryable<MessageWithDetails> messageListQuery = ApplyDataFilters<IMongoQueryable<Message>, Message>(userMessageQueryable.Join(chatMongoDbContext.ChatMessages, chatUserMessage => chatUserMessage.ChatMessageId, message => message.Id, (chatUserMessage, message) => new
            {
                chatUserMessage = chatUserMessage,
                message = message
            }).Where(data => userId == data.chatUserMessage.UserId && targetUserId == data.chatUserMessage.TargetUserId)
            .Select(data => data.message))
            .Select(p => new MessageWithDetails { Message = p });

            IMongoQueryable<UserMessage> userMessageListQuery = userMessageQueryable.Where(x => x.UserId == userId && x.TargetUserId == targetUserId);
            List<MessageWithDetails> chatMessageWithDetailsList = await messageListQuery.OrderByDescending(x => x.Message.CreationTime)
                .PageBy<MessageWithDetails, IMongoQueryable<MessageWithDetails>>(0, 1).ToListAsync(GetCancellationToken(cancellationToken));
            List<UserMessage> userMessageList = await userMessageListQuery.PageBy<UserMessage, IMongoQueryable<UserMessage>>(0, 1).ToListAsync(GetCancellationToken(cancellationToken));
            foreach (MessageWithDetails messageWithDetails in chatMessageWithDetailsList)
            {
                messageWithDetails.UserMessage = userMessageList.Find(x => x.ChatMessageId == messageWithDetails.Message.Id);
            }

            return chatMessageWithDetailsList.FirstOrDefault();
        }

        public async Task<List<Guid>> GetAllMessageIdsAsync(
          Guid userId,
          Guid targetUserId,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .Where(message => (message.UserId == userId && message.TargetUserId == targetUserId) || (message.UserId == targetUserId && message.TargetUserId == userId))
                .Select(message => message.ChatMessageId)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task DeleteAllMessages(
          Guid userId,
          Guid targetUserId,
          CancellationToken cancellationToken = default)
        {
            List<Guid> ids = await (await GetMongoQueryableAsync(cancellationToken, null))
                .Where(message => (message.UserId == userId && message.TargetUserId == targetUserId) || (message.UserId == targetUserId && message.TargetUserId == userId))
                .Select(message => message.Id)
                .ToListAsync(GetCancellationToken(cancellationToken));
            await DeleteManyAsync(ids, false, cancellationToken);
        }

        public virtual async Task<bool> HasConversationAsync(
          Guid userId,
          Guid targetUserId,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null)).AnyAsync(p => p.UserId == userId && p.TargetUserId == targetUserId, GetCancellationToken(cancellationToken));
        }

        public async Task<List<UserMessage>> GetListAsync(
          Guid messageId,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .Where(message => message.ChatMessageId == messageId)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
