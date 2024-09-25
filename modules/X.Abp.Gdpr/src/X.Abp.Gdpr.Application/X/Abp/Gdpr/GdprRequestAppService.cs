// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Gdpr;
using Volo.Abp.Users;

namespace X.Abp.Gdpr;

[Authorize]
public class GdprRequestAppService : GdprAppServiceBase, IGdprRequestAppService
{
    protected IGdprRequestRepository GdprRequestRepository { get; }

    protected IDistributedEventBus EventBus { get; }

    protected AbpGdprOptions GdprOptions { get; }

    protected IDistributedCache<DownloadTokenCacheItem, string> DownloadTokenCache { get; }

    public GdprRequestAppService(
      IGdprRequestRepository gdprRequestRepository,
      IDistributedEventBus eventBus,
      IOptions<AbpGdprOptions> gdprOptions,
      IDistributedCache<DownloadTokenCacheItem, string> downloadTokenCache)
    {
        GdprRequestRepository = gdprRequestRepository;
        EventBus = eventBus;
        GdprOptions = gdprOptions.Value;
        DownloadTokenCache = downloadTokenCache;
    }

    public virtual async Task PrepareUserDataAsync()
    {
        if (!await IsNewRequestAllowedInternalAsync())
        {
            throw new BusinessException(GdprErrorCodes.NotAllowedForRequest);
        }

        var gdprRequest = new GdprRequest(GuidGenerator.Create(), CurrentUser.GetId(), Clock.Now.AddMinutes(GdprOptions.MinutesForDataPreparation));
        await GdprRequestRepository.InsertAsync(gdprRequest);
        await EventBus.PublishAsync(new GdprUserDataRequestedEto()
        {
            UserId = gdprRequest.UserId,
            RequestId = gdprRequest.Id
        });
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetUserDataAsync(Guid requestId, string token)
    {
        var downloadTokenCacheItem = await DownloadTokenCache.GetAsync(token);
        if (downloadTokenCacheItem == null || downloadTokenCacheItem.RequestId != requestId)
        {
            throw new AbpAuthorizationException();
        }

        await DownloadTokenCache.RemoveAsync(token);
        var gdprRequest = await GdprRequestRepository.GetAsync(requestId);
        if (Clock.Now < gdprRequest.ReadyTime)
        {
            throw new BusinessException(GdprErrorCodes.DataNotPreparedYet).WithData("GdprDataReadyTime", gdprRequest.ReadyTime.ToShortTimeString());
        }

        using var memoryStream = new MemoryStream();
        using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var info in gdprRequest.Infos)
            {
                using var entry = archive.CreateEntry(GuidGenerator.Create().ToString() + ".json", CompressionLevel.Fastest).Open();
                var bytes = Encoding.UTF8.GetBytes(info.Data);
                await entry.WriteAsync(bytes.AsMemory(0, bytes.Length));
            }
        }

        memoryStream.Seek(0L, SeekOrigin.Begin);
        var ms = new MemoryStream();
        await memoryStream.CopyToAsync(ms);
        ms.Seek(0L, SeekOrigin.Begin);
        return new RemoteStreamContent(ms, "PersonalData.zip", "application/zip");
    }

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync(Guid requestId)
    {
        if ((await GdprRequestRepository.GetAsync(requestId)).UserId != CurrentUser.GetId())
        {
            throw new UserFriendlyException(L["CanNotGetDownloadToken"]);
        }

        var token = Guid.NewGuid().ToString();
        var downloadTokenCache = DownloadTokenCache;

        var downloadTokenCacheItem = new DownloadTokenCacheItem
        {
            RequestId = requestId
        };
        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan?(TimeSpan.FromMinutes(60.0))
        };

        await downloadTokenCache.SetAsync(token, downloadTokenCacheItem, cacheEntryOptions);
        var downloadTokenResult = new DownloadTokenResultDto()
        {
            Token = token
        };

        return downloadTokenResult;
    }

    public virtual async Task<bool> IsNewRequestAllowedAsync()
    {
        return await IsNewRequestAllowedInternalAsync();
    }

    public virtual async Task<PagedResultDto<GdprRequestDto>> GetListByUserIdAsync(Guid userId)
    {
        if (userId != CurrentUser.GetId())
        {
            throw new AbpAuthorizationException();
        }

        var count = await GdprRequestRepository.GetCountByUserIdAsync(userId);
        var gdprRequestList = await GdprRequestRepository.GetListByUserIdAsync(userId);
        return new PagedResultDto<GdprRequestDto>(count, ObjectMapper.Map<List<GdprRequest>, List<GdprRequestDto>>(gdprRequestList));
    }

    public virtual async Task DeleteUserDataAsync()
    {
        var userId = CurrentUser.GetId();
        var gdprRequestList = await GdprRequestRepository.GetListByUserIdAsync(userId);
        await GdprRequestRepository.DeleteManyAsync(gdprRequestList, true);

        await EventBus.PublishAsync(new GdprUserDataDeletionRequestedEto()
        {
            UserId = userId
        });
    }

    private async Task<bool> IsNewRequestAllowedInternalAsync()
    {
        var latestRequestTime = await GdprRequestRepository.FindLatestRequestTimeOfUserAsync(CurrentUser.GetId());
        return latestRequestTime.HasValue && (Clock.Now - latestRequestTime) > GdprOptions.RequestTimeInterval;
    }
}
