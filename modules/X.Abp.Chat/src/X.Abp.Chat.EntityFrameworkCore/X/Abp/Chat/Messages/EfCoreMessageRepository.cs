﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
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

public class EfCoreMessageRepository : EfCoreRepository<IChatDbContext, Message, Guid>, IMessageRepository
{
    public EfCoreMessageRepository(IDbContextProvider<IChatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task DeleteALlMessagesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        await (await GetDbSetAsync()).Where(message => ids.Contains(message.Id)).ExecuteDeleteAsync(GetCancellationToken(cancellationToken));
    }
}
