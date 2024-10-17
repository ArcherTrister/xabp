// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.MongoDB.Messages
{
    public class MongoMessageRepository : MongoDbRepository<IChatMongoDbContext, Message, Guid>, IMessageRepository
    {
        public MongoMessageRepository(IMongoDbContextProvider<IChatMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public async Task DeleteALlMessagesAsync(
          IEnumerable<Guid> ids,
          CancellationToken cancellationToken = default)
        {
            await DeleteManyAsync(ids, false, cancellationToken);
        }
    }
}
