// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace X.Abp.LanguageManagement;

public class LanguageTextCacheInvalidator : ILocalEventHandler<EntityChangedEventData<LanguageText>>, ITransientDependency
{
    protected IDistributedCache<LanguageTextCacheItem> Cache { get; }

    public LanguageTextCacheInvalidator(IDistributedCache<LanguageTextCacheItem> cache)
    {
        Cache = cache;
    }

    public virtual async Task HandleEventAsync(EntityChangedEventData<LanguageText> eventData)
    {
        await Cache.RemoveAsync(
            LanguageTextCacheItem.CalculateCacheKey(eventData.Entity.ResourceName,
                eventData.Entity.CultureName));
    }
}
