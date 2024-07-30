// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace X.Abp.LanguageManagement;

public class LanguageListCacheInvalidator : ILocalEventHandler<EntityChangedEventData<Language>>, ITransientDependency
{
    protected IDistributedCache<LanguageListCacheItem> Cache { get; }

    public LanguageListCacheInvalidator(IDistributedCache<LanguageListCacheItem> cache)
    {
        Cache = cache;
    }

    public virtual async Task HandleEventAsync(EntityChangedEventData<Language> eventData)
    {
        await Cache.RemoveAsync(DatabaseLanguageProvider.CacheKey);
    }
}
