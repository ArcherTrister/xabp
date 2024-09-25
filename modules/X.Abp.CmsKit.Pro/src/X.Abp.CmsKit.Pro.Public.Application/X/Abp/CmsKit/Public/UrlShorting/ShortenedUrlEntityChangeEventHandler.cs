// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Public.UrlShorting;

public class ShortenedUrlEntityChangeEventHandler :
  ILocalEventHandler<EntityUpdatedEventData<ShortenedUrl>>,
  ILocalEventHandler<EntityDeletedEventData<ShortenedUrl>>,
  ITransientDependency
{
  protected IDistributedCache<ShortenedUrlCacheItem, string> DistributedCache { get; }

  public ShortenedUrlEntityChangeEventHandler(
    IDistributedCache<ShortenedUrlCacheItem, string> shortenedUrlCache)
  {
    DistributedCache = shortenedUrlCache;
  }

  public virtual async Task HandleEventAsync(EntityUpdatedEventData<ShortenedUrl> eventData)
  {
    await RemoveShortenedUrlAsync(eventData.Entity);
  }

  public virtual async Task HandleEventAsync(EntityDeletedEventData<ShortenedUrl> eventData)
  {
    await RemoveShortenedUrlAsync(eventData.Entity);
  }

  private async Task RemoveShortenedUrlAsync(ShortenedUrl shortenedUrl)
  {
    await DistributedCache.RemoveAsync(shortenedUrl.Source);
  }
}
