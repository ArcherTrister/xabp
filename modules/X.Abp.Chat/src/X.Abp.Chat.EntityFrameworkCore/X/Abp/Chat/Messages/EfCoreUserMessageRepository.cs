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

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.EntityFrameworkCore.Messages;

public class EfCoreUserMessageRepository : EfCoreRepository<IChatDbContext, UserMessage, Guid>, IUserMessageRepository
{
    public EfCoreUserMessageRepository(IDbContextProvider<IChatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MessageWithDetails>> GetMessagesAsync(Guid userId, Guid targetUserId, int skipCount, int maxResultCount, CancellationToken cancellationToken = default)
    {
        var query = from chatUserMessage in await GetDbSetAsync()
                    join message in (await GetDbContextAsync()).ChatMessages on chatUserMessage.ChatMessageId equals message.Id
                    where userId == chatUserMessage.UserId && targetUserId == chatUserMessage.TargetUserId
                    select new MessageWithDetails
                    {
                        UserMessage = chatUserMessage,
                        Message = message
                    };

        return await query.OrderByDescending(x => x.Message.CreationTime).PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }
}
