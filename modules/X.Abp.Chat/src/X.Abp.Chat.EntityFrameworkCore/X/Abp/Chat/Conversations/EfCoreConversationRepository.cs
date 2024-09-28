// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;

namespace X.Abp.Chat.EntityFrameworkCore.Conversations;

public class EfCoreConversationRepository : EfCoreRepository<IChatDbContext, Conversation, Guid>, IConversationRepository
{
  public EfCoreConversationRepository(IDbContextProvider<IChatDbContext> dbContextProvider)
      : base(dbContextProvider)
  {
  }

  public virtual async Task<ConversationPair> FindPairAsync(Guid senderId, Guid targetId, CancellationToken cancellationToken = default)
  {
    var matchedConversations = await (await GetDbSetAsync())
        .Where(x => (x.UserId == senderId && x.TargetUserId == targetId) ||
                    (x.UserId == targetId && x.TargetUserId == senderId)).ToListAsync(GetCancellationToken(cancellationToken));

    return matchedConversations.Count == 0
        ? null
        : new ConversationPair
        {
          SenderConversation = matchedConversations.Single(x => x.UserId == senderId),
          TargetConversation = matchedConversations.Single(x => x.UserId == targetId)
        };
  }

  public virtual async Task<List<ConversationWithTargetUser>> GetListByUserIdAsync(Guid userId, string filter, CancellationToken cancellationToken = default)
  {
    var query = from chatConversation in await GetDbSetAsync()
                join targetUser in (await GetDbContextAsync()).ChatUsers on chatConversation.TargetUserId equals targetUser.Id
                where userId == chatConversation.UserId && (targetUser.Name.Contains(filter) || targetUser.Surname.Contains(filter))
                orderby chatConversation.LastMessageDate descending
                select new ConversationWithTargetUser
                {
                  Conversation = chatConversation,
                  TargetUser = targetUser
                };

    return await query.ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<int> GetTotalUnreadMessageCountAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await (await GetQueryableAsync())
        .Where(x => x.UserId == userId && x.LastMessageSide == ChatMessageSide.Receiver)
        .SumAsync(x => x.UnreadMessageCount, cancellationToken: GetCancellationToken(cancellationToken));
  }
}
