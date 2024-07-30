// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

using X.Abp.VersionManagement.AppEditions.Dtos;

namespace X.Abp.VersionManagement.AppEditions;

/// <summary>
/// 应用程序版本管理服务
/// </summary>
public interface IAppEditionAppService : IApplicationService
{
    /// <summary>
    /// Android应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="currentVersion">当前版本</param>
    Task<CheckUpdateOutput> CheckUpdateAndroidAsync(string appName, string arch, string channel, string currentVersion);

    /// <summary>
    /// Windows应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="fileName">yml文件名</param>
    Task<string> CheckUpdateWinAsync(string appName, string arch, string channel, string fileName);

    /// <summary>
    /// Mac应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="fileName">yml文件名</param>
    Task<string> CheckUpdateMacAsync(string appName, string arch, string channel, string fileName);

    /// <summary>
    /// 上传应用程序
    /// </summary>
    /// <param name="input">input</param>
    Task<AppUploaderOutput> UploadApplicationAsync(UploadApplicationInputWithStream input);

    /// <summary>
    /// 下载应用程序
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="platform">平台</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    Task<IRemoteStreamContent> DownloaderAsync(string appName, string platform, string arch, string channel);

    /// <summary>
    /// 新增应用程序版本信息
    /// </summary>
    /// <param name="input">input</param>
    Task<AppEditionDto> CreateAsync(CreateAppEditionDto input);

    /// <summary>
    /// 删除应用程序版本
    /// </summary>
    /// <param name="id">id</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 应用程序版本详情
    /// </summary>
    /// <param name="id">id</param>
    Task<AppEditionDto> GetAsync(Guid id);

    /// <summary>
    /// 应用程序版本信息列表
    /// </summary>
    /// <param name="input">input</param>
    Task<PagedResultDto<AppEditionDto>> GetListAsync(PagedAppEditionDto input);

    /// <summary>
    /// 更新应用程序版本信息
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="input">input</param>
    Task UpdateAsync(Guid id, UpdateAppEditionDto input);

    /// <summary>
    /// 发布应用程序
    /// </summary>
    /// <param name="id">id</param>
    Task SetPublishedAsync(Guid id);
}
