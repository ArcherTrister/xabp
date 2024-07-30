// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Features;

using X.Abp.VersionManagement.AppEditions.Dtos;
using X.Abp.VersionManagement.Permissions;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Version = System.Version;

namespace X.Abp.VersionManagement.AppEditions;

/// <summary>
/// 应用程序版本管理服务
/// </summary>
[RequiresFeature(VersionManagementFeatures.Enable)]
[Authorize(AbpVersionManagementPermissions.AppEditions.Default)]
public class AppEditionAppService : VersionManagementAppServiceBase, IAppEditionAppService
{
    protected IAppEditionRepository AppEditionRepository => LazyServiceProvider.LazyGetRequiredService<IAppEditionRepository>();

    protected IBlobContainer<ApplicationFileContainer> BlobContainer => LazyServiceProvider.LazyGetRequiredService<IBlobContainer<ApplicationFileContainer>>();

    // protected IHttpContextAccessor HttpContextAccessor => LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();
    protected IDistributedEventBus DistributedEventBus => LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();

    // private static readonly string[] FileTypes = new string[] { ".exe", ".zip", ".dmg", ".pkg", ".mas", ".deb", ".rpm", ".yml", ".blockmap" };
    private static readonly string[] InsertFileTypes = new string[] { ".exe", ".zip", ".dmg", ".pkg", ".mas", ".deb", ".rpm", ".apk" };

    /// <summary>
    /// Android应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="currentVersion">当前版本</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [AllowAnonymous]
    public virtual async Task<CheckUpdateOutput> CheckUpdateAndroidAsync(string appName, string arch, string channel, string currentVersion)
    {
        var appEdition = await AppEditionRepository.CheckUpdateAsync(appName, PlatformType.Android, arch, channel);
        if (appEdition == null)
        {
            // 不需要更新
            return new CheckUpdateOutput { IsNeedUpdate = false, ForceUpdate = null };
        }
        else
        {
            var appVersion = new Version(currentVersion);
            var lastVersion = new Version(appEdition.Version);
            if (lastVersion > appVersion)
            {
                // 需要更新
                if (lastVersion.Major - appVersion.Major > 0)
                {
                    return new CheckUpdateOutput { IsNeedUpdate = true, ForceUpdate = new ForceUpdateDto { IsForceUpdate = true, DownloadPath = appEdition.DownloadPath, UpdateContent = appEdition.UpdateContent, CurrentVersion = appEdition.Version.ToString() } };
                }

                if (lastVersion.Minor - appVersion.Minor > 0)
                {
                    return new CheckUpdateOutput { IsNeedUpdate = true, ForceUpdate = new ForceUpdateDto { IsForceUpdate = true, DownloadPath = appEdition.DownloadPath, UpdateContent = appEdition.UpdateContent, CurrentVersion = appEdition.Version.ToString() } };
                }

                // if (currentVersion.Build - appVersion.Build > 0)
                // {
                //     return new CheckUpdateOutput { IsNeedUpdate = true, ForceUpdate = new ForceUpdateDto { IsForceUpdate = true, AndroidApkDownloadUrl = appEdition.AndroidApkDownloadUrl, UpdateContent = appEdition.UpdateContent } };
                // }
                return new CheckUpdateOutput { IsNeedUpdate = true, ForceUpdate = new ForceUpdateDto { IsForceUpdate = false, DownloadPath = appEdition.DownloadPath, UpdateContent = appEdition.UpdateContent, CurrentVersion = appEdition.Version.ToString() } };
            }
            else
            {
                // 不需要更新
                return new CheckUpdateOutput { IsNeedUpdate = false, ForceUpdate = null };
            }
        }
    }

    /// <summary>
    /// Windows应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="fileName">yml文件名</param>
    [AllowAnonymous]
    public virtual async Task<string> CheckUpdateWinAsync(string appName, string arch, string channel, string fileName)
    {
        var fileExt = Path.GetExtension(fileName);
        if (fileExt.Equals(".yml", StringComparison.OrdinalIgnoreCase))
        {
            var appEdition = await AppEditionRepository.CheckUpdateAsync(appName, PlatformType.Windows, arch, channel);
            if (appEdition != null)
            {
                var electronAppInfo = new ElectronAppInfo
                {
                    Files = new List<ElectronAppInfo.FilesItem>
                    {
                        new ElectronAppInfo.FilesItem
                        {
                            Sha512 = appEdition.Hash,
                            Size = appEdition.FileSize,
                            Url = appEdition.DownloadPath,
                            IsAdminRightsRequired = appEdition.IsAdminRightsRequired,
                        }
                    },
                    Sha512 = appEdition.Hash,
                    Path = appEdition.DownloadPath,
                    ReleaseDate = appEdition.ReleaseDate,
                    UpdateContent = appEdition.UpdateContent,
                    Version = $"{appEdition.Version}-{appEdition.Channel}"
                };
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var latestYml = serializer.Serialize(electronAppInfo);
                return latestYml;
            }
        }

        // return null;// new NotFoundResult();
        throw new UserFriendlyException("未找到此应用程序!");
    }

    /// <summary>
    /// Mac应用程序检查更新
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    /// <param name="fileName">yml文件名</param>
    [AllowAnonymous]
    public virtual async Task<string> CheckUpdateMacAsync(string appName, string arch, string channel, string fileName)
    {
        var fileExt = Path.GetExtension(fileName);
        if (fileExt.Equals(".yml", StringComparison.OrdinalIgnoreCase))
        {
            var appEdition = await AppEditionRepository.CheckUpdateAsync(appName, PlatformType.Windows, arch, channel);
            if (appEdition != null)
            {
                var electronAppInfo = new ElectronAppInfo
                {
                    Files = new List<ElectronAppInfo.FilesItem>
                    {
                        new ElectronAppInfo.FilesItem
                        {
                            Sha512 = appEdition.Hash,
                            Size = appEdition.FileSize,
                            Url = appEdition.DownloadPath,
                            IsAdminRightsRequired = appEdition.IsAdminRightsRequired,
                        }
                    },
                    Sha512 = appEdition.Hash,
                    Path = appEdition.DownloadPath,
                    ReleaseDate = appEdition.ReleaseDate,
                    UpdateContent = appEdition.UpdateContent,
                    Version = $"{appEdition.Version}-{appEdition.Channel}"
                };
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var latestMacYml = serializer.Serialize(electronAppInfo);
                return latestMacYml;
            }
        }

        // return null;// new NotFoundResult(); // new ContentResult { Content = latestMacYml };
        throw new UserFriendlyException("未找到此应用程序!");
    }

    /// <summary>
    /// 上传应用程序
    /// </summary>
    /// <param name="input">上传</param>
    [Authorize(AbpVersionManagementPermissions.AppEditions.Create)]
    public virtual async Task<AppUploaderOutput> UploadApplicationAsync(UploadApplicationInputWithStream input)
    {
        if (input.File.FileName.Length > 50)
        {
            throw new UserFriendlyException("文件名称过长,长度应小于等于50!");
        }

        var fileExt = Path.GetExtension(input.File.FileName);
        if (!InsertFileTypes.Contains(fileExt))
        {
            throw new UserFriendlyException("文件格式错误，请上传正确的桌面应用格式的文件!");
        }

        var mimeType = input.File.ContentType;

        // 'aix' | 'android' | 'darwin' | 'freebsd' | 'haiku' | 'linux' | 'openbsd' | 'sunos' | 'win32' | 'cygwin' | 'netbsd';
        // 'arm' | 'arm64' | 'ia32' | 'mips' | 'mipsel' | 'ppc' | 'ppc64' | 's390' | 's390x' | 'x64';
        // "alpha", "beta", "stable", "latest"
        string appName;
        string platform;
        string arch;
        string version;
        string channel;
        try
        {
            // 0:appName 2:arch 3:version 4:arch
            var folders = Path.GetFileNameWithoutExtension(input.File.FileName)
                .Split("-", StringSplitOptions.RemoveEmptyEntries);
            if (folders.Length == 4 && fileExt.Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                appName = folders[0];
                platform = folders[1];
                version = folders[2];
                channel = folders[3];
                arch = "winAll";
            }
            else
            {
                appName = folders[0];
                platform = folders[1];
                arch = folders[2];
                version = folders[3];
                channel = folders[4];
            }
        }
        catch (Exception)
        {
            throw new UserFriendlyException("文件名格式错误，例:【appName-platform-arch-version-channel.ext】!");
        }

        var platformType = AppEditionExtension.GetPlatformType(platform);

        var isReleased = await AppEditionRepository.CheckUploaderVersionAsync(appName, platformType, arch, channel, version);

        if (isReleased)
        {
            throw new UserFriendlyException("此版本的应用已发布!");
        }

        var uniqueFileName = AppEditionExtension.GetUniqueFileDownloadName(appName, platformType, arch, channel, version, fileExt);

        using var stream = input.File.GetStream();
        var position = input.File.GetStream().Position;

        // IsValidImage may change the position of the stream
        if (input.File.GetStream().CanSeek)
        {
            stream.Position = position;
        }

        var bytes = await stream.GetAllBytesAsync();
        var hash = AppEditionExtension.GetHash(bytes, "sha512", "base64");
        var fileSize = stream.Length;
        await BlobContainer.SaveAsync(uniqueFileName, bytes, true);

        AppUploaderOutput result = new()
        {
            AppName = appName,
            Version = version,
            Arch = arch,
            Channel = channel,
            PlatformType = platformType,
            UniqueFileName = uniqueFileName,
            FileExt = fileExt,
            FileSize = fileSize,
            Hash = hash,
            MimeType = mimeType
        };

        return result;
    }

    /// <summary>
    /// 下载应用程序
    /// </summary>
    /// <param name="appName">Application Name</param>
    /// <param name="platform">平台</param>
    /// <param name="arch">架构</param>
    /// <param name="channel">发行版</param>
    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> DownloaderAsync(string appName, string platform, string arch, string channel)
    {
        var platformType = AppEditionExtension.GetPlatformType(platform);
        var appEdition = await AppEditionRepository.GetLatestVersion(appName, platformType, arch, channel);

        if (appEdition != null)
        {
            var fileStream = await BlobContainer.GetAsync(appEdition.UniqueFileName);

            // return new FileStreamResult(fileStream, MimeMapping.MimeUtility.GetMimeMapping(appEdition.UniqueFileName))
            // {
            //     FileDownloadName = appEdition.UniqueFileName
            // };
            return new RemoteStreamContent(fileStream, appEdition.UniqueFileName, appEdition.MimeType);
        }

        throw new UserFriendlyException("未找到此文件!");
    }

    /// <summary>
    /// 新增应用程序版本信息
    /// </summary>
    /// <param name="input">input</param>
    [Authorize(AbpVersionManagementPermissions.AppEditions.Create)]
    public virtual async Task<AppEditionDto> CreateAsync(CreateAppEditionDto input)
    {
        Version v = new(input.Version);
        var appEdition = await AppEditionRepository.FindAsync(input.AppName, input.PlatformType, input.Arch, input.Channel, input.Version);
        if (appEdition != null)
        {
            if (appEdition.PublishType == PublishType.Released)
            {
                throw new UserFriendlyException("此版本的应用已发布!");
            }

            appEdition.FileSize = input.FileSize;
            appEdition.Hash = input.Hash;
            appEdition.UpdateContent = input.UpdateContent;
            appEdition.DownloadPath = $"/api/app-edition/download/{input.AppName}/{input.PlatformType}/{input.Arch}/{input.Channel}";
            appEdition.SetConcurrencyStampIfNotNull(appEdition.ConcurrencyStamp);
            return ObjectMapper.Map<AppEdition, AppEditionDto>(await AppEditionRepository.UpdateAsync(appEdition, autoSave: true));
        }
        else
        {
            var entity = new AppEdition
            {
                AppName = input.AppName,
                Version = input.Version,
                Major = v.Major,
                Minor = v.Minor,
                Build = v.Build,
                Revision = v.Revision,
                Arch = input.Arch,
                Channel = input.Channel,
                UniqueFileName = input.UniqueFileName,
                FileExt = input.FileExt,
                FileSize = input.FileSize,
                MimeType = input.MimeType,
                Hash = input.Hash,
                PlatformType = input.PlatformType,
                PublishType = PublishType.ToBeReleased,
                UpdateContent = input.UpdateContent,
                DownloadPath = $"/api/app-edition/download/{input.AppName}/{input.PlatformType}/{input.Arch}/{input.Channel}",
                ReleaseDate = Clock.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                IsAdminRightsRequired = false // input.PlatformType == PlatformType.Windows
            };

            return ObjectMapper.Map<AppEdition, AppEditionDto>(await AppEditionRepository.InsertAsync(entity, autoSave: true));
        }
    }

    /// <summary>
    /// 删除应用程序版本
    /// </summary>
    /// <param name="id">id</param>
    [Authorize(AbpVersionManagementPermissions.AppEditions.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var appEdition = await AppEditionRepository.FindAsync(id);
        if (appEdition == null)
        {
            throw new UserFriendlyException("未找到相关信息");
        }

        await BlobContainer.DeleteAsync(appEdition.UniqueFileName);
        await AppEditionRepository.DeleteAsync(id);
    }

    /// <summary>
    /// 应用程序版本详情
    /// </summary>
    /// <param name="id">id</param>
    [Authorize]
    public virtual async Task<AppEditionDto> GetAsync(Guid id)
    {
        var appEdition = await AppEditionRepository.GetAsync(id);
        return ObjectMapper.Map<AppEdition, AppEditionDto>(appEdition);
    }

    /// <summary>
    /// 应用程序版本信息列表
    /// </summary>
    /// <param name="input">input</param>
    [Authorize]
    public virtual async Task<PagedResultDto<AppEditionDto>> GetListAsync(PagedAppEditionDto input)
    {
        // : GetListPolicyName
        // await CheckPolicyAsync(GetListPolicyName);
        var list = await AppEditionRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.AppName, input.Arch, input.Version, input.Channel, input.PlatformType);
        var totalCount = await AppEditionRepository.GetCountAsync(input.AppName, input.Arch, input.Version, input.Channel, input.PlatformType);

        return new PagedResultDto<AppEditionDto>(
            totalCount,
            ObjectMapper.Map<List<AppEdition>, List<AppEditionDto>>(list));
    }

    /// <summary>
    /// 更新应用程序版本信息
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="input">input</param>
    [Authorize(AbpVersionManagementPermissions.AppEditions.Edit)]
    public virtual async Task UpdateAsync(Guid id, UpdateAppEditionDto input)
    {
        var appEdition = await AppEditionRepository.GetAsync(id);
        if (appEdition == null)
        {
            throw new UserFriendlyException("未找到相关信息");
        }

        if (appEdition.PublishType == PublishType.Released)
        {
            throw new UserFriendlyException("此应用已发布,无法更改!");
        }

        appEdition.UpdateContent = input.UpdateContent;
        appEdition.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        await AppEditionRepository.UpdateAsync(appEdition, autoSave: true);
    }

    /// <summary>
    /// 发布应用程序
    /// </summary>
    /// <param name="id">id</param>
    [Authorize(AbpVersionManagementPermissions.AppEditions.Edit)]
    public virtual async Task SetPublishedAsync(Guid id)
    {
        var appEdition = await AppEditionRepository.GetAsync(id);
        if (appEdition == null)
        {
            throw new UserFriendlyException("未找到相关信息");
        }

        appEdition.PublishType = PublishType.Released;
        appEdition.SetConcurrencyStampIfNotNull(appEdition.ConcurrencyStamp);
        await AppEditionRepository.UpdateAsync(appEdition, autoSave: true);
    }
}
