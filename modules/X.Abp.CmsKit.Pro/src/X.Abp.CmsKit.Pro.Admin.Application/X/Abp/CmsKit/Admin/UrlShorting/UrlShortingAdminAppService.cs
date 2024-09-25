// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Admin.UrlShorting;

[RequiresGlobalFeature("CmsKitPro.UrlShorting")]
[Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Default)]
public class UrlShortingAdminAppService : CmsKitProAdminAppServiceBase, IUrlShortingAdminAppService
{
  protected IShortenedUrlRepository ShortenedUrlRepository { get; }

  public UrlShortingAdminAppService(IShortenedUrlRepository shortenedUrlRepository)
  {
    ShortenedUrlRepository = shortenedUrlRepository;
  }

  public virtual async Task<PagedResultDto<ShortenedUrlDto>> GetListAsync(GetShortenedUrlListInput input)
  {
    var shortenedUrlList = await ShortenedUrlRepository.GetListAsync(input.ShortenedUrlFilter, input.Sorting, input.SkipCount, input.MaxResultCount);
    return new PagedResultDto<ShortenedUrlDto>(await ShortenedUrlRepository.GetCountAsync(input.ShortenedUrlFilter), ObjectMapper.Map<List<ShortenedUrl>, List<ShortenedUrlDto>>(shortenedUrlList));
  }

  public virtual async Task<ShortenedUrlDto> GetAsync(Guid id)
  {
    return ObjectMapper.Map<ShortenedUrl, ShortenedUrlDto>(await ShortenedUrlRepository.GetAsync(id, true));
  }

  [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Create)]
  public virtual async Task<ShortenedUrlDto> CreateAsync(CreateShortenedUrlDto input)
  {
    input.Source = input.Source.EnsureStartsWith('/', StringComparison.Ordinal);
    await ValidateShortenedUrlAsync(input.Source);
    var shortenedUrl = await ShortenedUrlRepository.InsertAsync(new ShortenedUrl(GuidGenerator.Create(), input.Source, input.Target, CurrentTenant?.Id), false);
    return ObjectMapper.Map<ShortenedUrl, ShortenedUrlDto>(shortenedUrl);
  }

  [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Update)]
  public virtual async Task<ShortenedUrlDto> UpdateAsync(Guid id, UpdateShortenedUrlDto input)
  {
    var shortenedUrl1 = await ShortenedUrlRepository.GetAsync(id, true);

    shortenedUrl1.SetTarget(input.Target);

    var shortenedUrl2 = await ShortenedUrlRepository.UpdateAsync(shortenedUrl1, false);

    return ObjectMapper.Map<ShortenedUrl, ShortenedUrlDto>(shortenedUrl2);
  }

  [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Delete)]
  public virtual async Task DeleteAsync(Guid id)
  {
    await ShortenedUrlRepository.DeleteAsync(id, false);
  }

  private async Task ValidateShortenedUrlAsync(string sourceUrl, Guid? shortenedUrlId = null)
  {
    var shortenedUrl = await ShortenedUrlRepository.FindBySourceUrlAsync(sourceUrl);
    if (shortenedUrl != null)
    {
      if (shortenedUrlId.HasValue)
      {
        if (!(shortenedUrl.Id != shortenedUrlId))
        {
          return;
        }
      }

      throw new ShortenedUrlAlreadyExistsException(sourceUrl);
    }
  }
}
