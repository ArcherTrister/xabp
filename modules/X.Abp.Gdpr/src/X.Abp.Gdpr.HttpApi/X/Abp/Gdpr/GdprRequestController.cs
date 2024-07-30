// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace X.Abp.Gdpr;

[Route("api/gdpr/requests")]
[ControllerName("GdprRequest")]
[Area(AbpGdprRemoteServiceConsts.ModuleName)]
[RemoteService(true, Name = AbpGdprRemoteServiceConsts.RemoteServiceName)]
public class GdprRequestController :
AbpControllerBase,
IGdprRequestAppService
{
    protected IGdprRequestAppService GdprRequestAppService { get; }

    public GdprRequestController(IGdprRequestAppService gdprRequestAppService)
    {
        GdprRequestAppService = gdprRequestAppService;
    }

    [HttpPost("prepare-data")]
    public virtual Task PrepareUserDataAsync()
    {
        return GdprRequestAppService.PrepareUserDataAsync();
    }

    [HttpGet("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid requestId)
    {
        return GdprRequestAppService.GetDownloadTokenAsync(requestId);
    }

    [HttpGet("data/{requestId}")]
    public virtual async Task<IRemoteStreamContent> GetUserDataAsync(Guid requestId, string token)
    {
        var userDataAsync = await GdprRequestAppService.GetUserDataAsync(requestId, token);
        Response.Headers.Add("Content-Disposition", "attachment;filename=\"" + userDataAsync.FileName + "\"");
        Response.Headers.Add("Accept-Ranges", "bytes");
        Response.ContentType = userDataAsync.ContentType;
        return userDataAsync;
    }

    [HttpGet("is-request-allowed")]
    public virtual Task<bool> IsNewRequestAllowedAsync()
    {
        return GdprRequestAppService.IsNewRequestAllowedAsync();
    }

    [HttpGet("list")]
    public virtual Task<PagedResultDto<GdprRequestDto>> GetListByUserIdAsync(Guid userId)
    {
        return GdprRequestAppService.GetListByUserIdAsync(userId);
    }

    [HttpDelete]
    public virtual Task DeleteUserDataAsync()
    {
        return GdprRequestAppService.DeleteUserDataAsync();
    }
}
