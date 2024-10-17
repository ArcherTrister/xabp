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
using X.Abp.Chat.MongoDB;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.Conversations
{
    public class MongoConversationRepository : MongoDbRepository<IChatMongoDbContext, Conversation, Guid>, IConversationRepository
    {
        public MongoConversationRepository(IMongoDbContextProvider<IChatMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<ConversationPair> FindPairAsync(
          Guid senderId,
          Guid targetId,
          CancellationToken cancellationToken = default)
        {
            var source = await (await GetMongoQueryableAsync(cancellationToken, null)).Where(x => (x.UserId == senderId && x.TargetUserId == targetId) || (x.UserId == targetId && x.TargetUserId == senderId))
                .ToListAsync(GetCancellationToken(cancellationToken));
            if (source.Count == 0)
            {
                return null;
            }

            return new ConversationPair()
            {
                SenderConversation = source.SingleOrDefault(x => x.UserId == senderId),
                TargetConversation = source.SingleOrDefault(x => x.UserId == targetId)
            };
        }

        public virtual async Task<List<ConversationWithTargetUser>> GetListByUserIdAsync(
          Guid userId,
          string filter,
          CancellationToken cancellationToken = default)
        {
            var conversationQueryable = await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null);
            var chatMongoDbContext = await GetDbContextAsync(GetCancellationToken(cancellationToken));
            var query = conversationQueryable
                .Join(chatMongoDbContext.ChatUsers, chatConversation => chatConversation.TargetUserId, targetUser => targetUser.Id, (chatConversation, targetUser) => new
                {
                    chatConversation,
                    targetUser
                }).Where(data => userId == data.chatConversation.UserId && (filter == default || filter == "" || data.targetUser.Name.Contains(filter) || data.targetUser.Surname.Contains(filter) || data.targetUser.UserName.Contains(filter)))
                .OrderByDescending(data => data.chatConversation.LastMessageDate)
                .Select(data => data.targetUser);

            var conversationsWithTargetDetails = await ApplyDataFilters<IMongoQueryable<ChatUser>, ChatUser>(query)
                .Select(p => new ConversationWithTargetUser
                {
                    TargetUser = p
                }).ToListAsync(GetCancellationToken(cancellationToken));

            var source = await conversationQueryable.Where(x => x.UserId == userId).ToListAsync(GetCancellationToken(cancellationToken));
            foreach (var conversationWithTargetUser in conversationsWithTargetDetails)
            {
                conversationWithTargetUser.Conversation = source.Single(x => x.TargetUserId == conversationWithTargetUser.TargetUser.Id);
            }

            return conversationsWithTargetDetails;
        }

        public virtual async Task<int> GetTotalUnreadMessageCountAsync(
          Guid userId,
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .Where(x => x.UserId == userId && x.LastMessageSide == ChatMessageSide.Receiver)
                .SumAsync(x => x.UnreadMessageCount, GetCancellationToken(cancellationToken));
        }
    }
}
