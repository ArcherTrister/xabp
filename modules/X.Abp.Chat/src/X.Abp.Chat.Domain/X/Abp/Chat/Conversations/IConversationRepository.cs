﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Chat.Conversations;

public interface IConversationRepository : IBasicRepository<Conversation, Guid>
{
    Task<ConversationPair> FindPairAsync(Guid senderId, Guid targetId, CancellationToken cancellationToken = default);

    Task<List<ConversationWithTargetUser>> GetListByUserIdAsync(Guid userId, string filter, CancellationToken cancellationToken = default);

    Task<int> GetTotalUnreadMessageCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
