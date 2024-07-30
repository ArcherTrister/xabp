// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

using X.Abp.VersionManagement.AppEditions.Dtos;

namespace X.Abp.VersionManagement.AppEditions;

/// <summary>
/// 应用程序版本管理
/// </summary>
[RemoteService(Name = AbpVersionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpVersionManagementRemoteServiceConsts.ModuleName)]
[Route("api/app-edition")]
public class AppEditionController : VersionManagementController, IAppEditionAppService
{
    protected IAppEditionAppService AppEditionAppService { get; }

    public AppEditionController(IAppEditionAppService appEditionAppService)
    {
        AppEditionAppService = appEditionAppService;
    }

    [HttpGet]
    [Route("check-update-android/{appName}/{arch}/{channel}/{currentVersion}")]
    public virtual async Task<CheckUpdateOutput> CheckUpdateAndroidAsync(string appName, string arch, string channel, string currentVersion)
    {
        return await AppEditionAppService.CheckUpdateAndroidAsync(appName, arch, channel, currentVersion);
    }

    [HttpGet]
    [Route("check-update-win/{appName}/{arch}/{channel}/{fileName?}")]
    public virtual async Task<string> CheckUpdateWinAsync(string appName, string arch, string channel, string fileName)
    {
        return await AppEditionAppService.CheckUpdateWinAsync(appName, arch, channel, fileName);
    }

    [HttpGet]
    [Route("check-update-mac/{appName}/{arch}/{fileName?}")]
    public virtual async Task<string> CheckUpdateMacAsync(string appName, string arch, string channel, string fileName)
    {
        return await AppEditionAppService.CheckUpdateMacAsync(appName, arch, channel, fileName);
    }

    // [DisableRequestSizeLimit]
    // [DisableFormValueModelBinding]
    [HttpPost]
    [RequestSizeLimit(1000_000_000)]
    [Route("upload-application")]
    public virtual async Task<AppUploaderOutput> UploadApplicationAsync([FromForm] UploadApplicationInputWithStream input)
    {
        return await AppEditionAppService.UploadApplicationAsync(input);
    }

    [HttpGet]
    [Route("download/{appName}/{platform}/{arch}/{channel}")]
    public virtual async Task<IRemoteStreamContent> DownloaderAsync(string appName, string platform, string arch, string channel)
    {
        return await AppEditionAppService.DownloaderAsync(appName, platform, arch, channel);
    }

    [HttpPost]
    public virtual async Task<AppEditionDto> CreateAsync(CreateAppEditionDto input)
    {
        return await AppEditionAppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await AppEditionAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<AppEditionDto> GetAsync(Guid id)
    {
        return await AppEditionAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<AppEditionDto>> GetListAsync(PagedAppEditionDto input)
    {
        return await AppEditionAppService.GetListAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task UpdateAsync(Guid id, UpdateAppEditionDto input)
    {
        await AppEditionAppService.UpdateAsync(id, input);
    }

    [HttpPut]
    [Route("set-published/{id}")]
    public virtual async Task SetPublishedAsync(Guid id)
    {
        await AppEditionAppService.SetPublishedAsync(id);
    }
}
