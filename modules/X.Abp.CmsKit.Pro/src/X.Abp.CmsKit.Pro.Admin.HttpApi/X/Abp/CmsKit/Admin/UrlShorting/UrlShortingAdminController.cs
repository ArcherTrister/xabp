// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.Admin;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.UrlShorting;

[Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Default)]
[Area(AbpCmsKitProAdminRemoteServiceConsts.ModuleName)]
[RemoteService(true, Name = CmsKitAdminRemoteServiceConsts.RemoteServiceName)]
[RequiresGlobalFeature(typeof(UrlShortingFeature))]
[Route("api/cms-kit-admin/url-shorting")]
public class UrlShortingAdminController :
  CmsKitProAdminController,
  IUrlShortingAdminAppService
{
    protected IUrlShortingAdminAppService UrlShortingAdminAppService { get; }

    public UrlShortingAdminController(IUrlShortingAdminAppService urlShortingAdminAppService)
    {
        UrlShortingAdminAppService = urlShortingAdminAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<ShortenedUrlDto>> GetListAsync(GetShortenedUrlListInput input)
    {
        return UrlShortingAdminAppService.GetListAsync(input);
    }

    [Route("{id}")]
    [HttpGet]
    public Task<ShortenedUrlDto> GetAsync(Guid id)
    {
        return UrlShortingAdminAppService.GetAsync(id);
    }

    [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Create)]
    [HttpPost]
    public Task<ShortenedUrlDto> CreateAsync(CreateShortenedUrlDto input)
    {
        return UrlShortingAdminAppService.CreateAsync(input);
    }

    [Route("{id}")]
    [HttpPut]
    [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Update)]
    public Task<ShortenedUrlDto> UpdateAsync(Guid id, UpdateShortenedUrlDto input)
    {
        return UrlShortingAdminAppService.UpdateAsync(id, input);
    }

    [Authorize(AbpCmsKitProAdminPermissions.UrlShorting.Delete)]
    [Route("{id}")]
    [HttpDelete]
    public Task DeleteAsync(Guid id)
    {
        return UrlShortingAdminAppService.DeleteAsync(id);
    }
}
