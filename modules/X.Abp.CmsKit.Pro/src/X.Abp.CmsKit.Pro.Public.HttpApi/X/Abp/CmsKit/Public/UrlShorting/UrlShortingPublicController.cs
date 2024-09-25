// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.UrlShorting;

[RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
[RequiresGlobalFeature(typeof(UrlShortingFeature))]
[Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
[Route("api/cms-kit-public/url-shorting")]
public class UrlShortingPublicController :
CmsKitProPublicController,
IUrlShortingPublicAppService
{
  protected IUrlShortingPublicAppService UrlShortingPublicAppService { get; }

  public UrlShortingPublicController(IUrlShortingPublicAppService urlShortingPublicAppService)
  {
    UrlShortingPublicAppService = urlShortingPublicAppService;
  }

  [HttpGet]
  public virtual async Task<ShortenedUrlDto> FindBySourceAsync(string source)
  {
    return await UrlShortingPublicAppService.FindBySourceAsync(source);
  }
}
