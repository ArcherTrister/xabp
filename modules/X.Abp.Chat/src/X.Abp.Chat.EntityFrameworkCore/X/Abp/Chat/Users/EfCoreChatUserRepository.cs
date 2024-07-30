// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users.EntityFrameworkCore;

using X.Abp.Chat.Users;

namespace X.Abp.Chat.EntityFrameworkCore.Users;

public class EfCoreChatUserRepository : EfCoreUserRepositoryBase<IChatDbContext, ChatUser>, IChatUserRepository
{
    public EfCoreChatUserRepository(IDbContextProvider<IChatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
