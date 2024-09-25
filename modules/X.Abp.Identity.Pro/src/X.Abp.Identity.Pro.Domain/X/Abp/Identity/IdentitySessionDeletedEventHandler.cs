// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace X.Abp.Identity;

public class IdentitySessionDeletedEventHandler :
  IDistributedEventHandler<EntityDeletedEventData<IdentitySession>>,
  ITransientDependency
{
    protected IDistributedCache<IdentitySessionCacheItem> Cache { get; }

    public IdentitySessionDeletedEventHandler(IDistributedCache<IdentitySessionCacheItem> cache)
    {
        Cache = cache;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(EntityDeletedEventData<IdentitySession> eventData)
    {
        await Cache.RemoveAsync(eventData.Entity.SessionId);
    }
}
