﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace X.Abp.Chat.Users;

public class ChatUserLookupService : UserLookupService<ChatUser, IChatUserRepository>, IChatUserLookupService
{
    public ChatUserLookupService(
        IChatUserRepository userRepository,
        IUnitOfWorkManager unitOfWorkManager)
        : base(userRepository, unitOfWorkManager)
    {
    }

    protected override ChatUser CreateUser(IUserData externalUser)
    {
        return new ChatUser(externalUser);
    }
}
