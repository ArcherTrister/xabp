// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.Users;
using X.Abp.Chat.Permission;
using X.Abp.Chat.Users;

namespace X.Abp.Chat;

[Authorize(AbpChatPermissions.Messaging)]
public class ChatHub : AbpHub
{
    protected IContactAppService ContactAppService { get; }

    protected IDistributedCache<string, Guid> TotalUnreadMessageCountCache { get; }

    public ChatHub(IContactAppService contactAppService, IDistributedCache<string, Guid> totalUnreadMessageCountCache)
    {
        ContactAppService = contactAppService;
        TotalUnreadMessageCountCache = totalUnreadMessageCountCache;
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // from cache
        var cacheItems = await TotalUnreadMessageCountCache.GetOrAddAsync(
         CurrentUser.GetId(),
         async () =>
         {
             var total = await ContactAppService.GetTotalUnreadMessageCountAsync();
             return total.ToString();
         },
         () => new DistributedCacheEntryOptions
         {
             AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
         });

        // from db
        await base.OnDisconnectedAsync(exception);
    }
}
