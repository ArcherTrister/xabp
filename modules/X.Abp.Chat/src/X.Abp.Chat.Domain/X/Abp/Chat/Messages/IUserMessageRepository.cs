// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Chat.Messages;

public interface IUserMessageRepository : IBasicRepository<UserMessage, Guid>
{
    Task<List<MessageWithDetails>> GetMessagesAsync(Guid userId, Guid targetUserId, int skipCount, int maxResultCount, CancellationToken cancellationToken = default);
}
