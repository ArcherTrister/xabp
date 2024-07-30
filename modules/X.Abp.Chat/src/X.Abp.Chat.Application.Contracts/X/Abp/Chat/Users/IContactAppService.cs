// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.Chat.Users;

public interface IContactAppService : IApplicationService
{
    Task<List<ChatContactDto>> GetContactsAsync(GetContactsInput input);

    Task<int> GetTotalUnreadMessageCountAsync();
}
