// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Public.UrlShorting;

[RequiresFeature(CmsKitProFeatures.UrlShortingEnable)]
[RequiresGlobalFeature(typeof(UrlShortingFeature))]
public class UrlShortingPublicAppService : PublicAppService, IUrlShortingPublicAppService
{
  protected IShortenedUrlRepository ShortenedUrlRepository { get; }

  protected IDistributedCache<ShortenedUrlCacheItem, string> DistributedCache { get; }

  public UrlShortingPublicAppService(
    IShortenedUrlRepository shortenedUrlRepository,
    IDistributedCache<ShortenedUrlCacheItem, string> shortenedUrlCache)
  {
    ShortenedUrlRepository = shortenedUrlRepository;
    DistributedCache = shortenedUrlCache;
  }

  public virtual async Task<ShortenedUrlDto> FindBySourceAsync(string source)
  {
    var val = await DistributedCache.GetAsync(source);
    if (val != null)
    {
      return !val.Exists ? null : ObjectMapper.Map<ShortenedUrlCacheItem, ShortenedUrlDto>(val);
    }

    var shortenedUrl = await ShortenedUrlRepository.FindBySourceUrlAsync(source);
    if (shortenedUrl == null)
    {
      await DistributedCache.SetAsync(source, new ShortenedUrlCacheItem
      {
        Exists = false
      });
      return null;
    }

    await DistributedCache.SetAsync(source, new ShortenedUrlCacheItem
    {
      Id = shortenedUrl.Id,
      Source = shortenedUrl.Source,
      Target = shortenedUrl.Target,
      Exists = true
    });
    return ObjectMapper.Map<ShortenedUrl, ShortenedUrlDto>(shortenedUrl);
  }
}
