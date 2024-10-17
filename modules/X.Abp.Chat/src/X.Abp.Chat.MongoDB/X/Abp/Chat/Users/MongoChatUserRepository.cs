// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.MongoDB;
using Volo.Abp.Users.MongoDB;

using X.Abp.Chat.Users;

namespace X.Abp.Chat.MongoDB.Users
{
    public class MongoChatUserRepository : MongoUserRepositoryBase<IChatMongoDbContext, ChatUser>, IChatUserRepository
    {
        public MongoChatUserRepository(IMongoDbContextProvider<IChatMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }
    }
}
