// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

namespace X.Abp.Chat.Users;

[RemoteService(Name = AbpChatRemoteServiceConsts.RemoteServiceName)]
[Area(AbpChatRemoteServiceConsts.ModuleName)]
[Route("api/chat/contact")]
public class ContactController : ChatController, IContactAppService
{
  private readonly IContactAppService _contactAppService;

  public ContactController(IContactAppService contactAppService)
  {
    _contactAppService = contactAppService;
  }

  [HttpGet]
  [Route("contacts")]
  public virtual Task<List<ChatContactDto>> GetContactsAsync(GetContactsInput input)
  {
    return _contactAppService.GetContactsAsync(input);
  }

  [HttpGet]
  [Route("total-unread-message-count")]
  public virtual Task<int> GetTotalUnreadMessageCountAsync()
  {
    return _contactAppService.GetTotalUnreadMessageCountAsync();
  }
}
