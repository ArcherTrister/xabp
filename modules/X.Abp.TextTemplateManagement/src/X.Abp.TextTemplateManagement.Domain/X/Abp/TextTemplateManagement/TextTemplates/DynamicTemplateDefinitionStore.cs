// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.TextTemplating;
using Volo.Abp.Threading;

namespace X.Abp.TextTemplateManagement.TextTemplates;

[Dependency(ReplaceServices = true)]
public class DynamicTemplateDefinitionStore : IDynamicTemplateDefinitionStore, ITransientDependency
{
    protected ITextTemplateDefinitionRecordRepository TextTemplateRepository { get; }

    protected ITextTemplateDefinitionSerializer TextTemplateDefinitionSerializer { get; }

    protected IDynamicTextTemplateDefinitionStoreInMemoryCache StoreCache { get; }

    protected IDistributedCache DistributedCache { get; }

    protected IAbpDistributedLock DistributedLock { get; }

    public TextTemplateManagementOptions TemplateManagementOptions { get; }

    protected AbpDistributedCacheOptions CacheOptions { get; }

    public DynamicTemplateDefinitionStore(
      ITextTemplateDefinitionRecordRepository textTemplateRepository,
      ITextTemplateDefinitionSerializer textTemplateDefinitionSerializer,
      IDynamicTextTemplateDefinitionStoreInMemoryCache storeCache,
      IDistributedCache distributedCache,
      IOptions<AbpDistributedCacheOptions> cacheOptions,
      IOptions<TextTemplateManagementOptions> templateManagementOptions,
      IAbpDistributedLock distributedLock)
    {
        TextTemplateRepository = textTemplateRepository;
        TextTemplateDefinitionSerializer = textTemplateDefinitionSerializer;
        StoreCache = storeCache;
        DistributedCache = distributedCache;
        DistributedLock = distributedLock;
        TemplateManagementOptions = templateManagementOptions.Value;
        CacheOptions = cacheOptions.Value;
    }

    public virtual async Task<TemplateDefinition> GetAsync(string name)
    {
        return await GetOrNullAsync(name) ?? throw new AbpException("Undefined template: " + name);
    }

    public virtual async Task<TemplateDefinition> GetOrNullAsync(string name)
    {
        if (!TemplateManagementOptions.IsDynamicTemplateStoreEnabled)
        {
            return null;
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetTemplateOrNull(name);
        }
    }

    public virtual async Task<IReadOnlyList<TemplateDefinition>> GetAllAsync()
    {
        if (!TemplateManagementOptions.IsDynamicTemplateStoreEnabled)
        {
            return Array.Empty<TemplateDefinition>();
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetTemplates().ToImmutableList();
        }
    }

    protected virtual async Task EnsureCacheIsUptoDateAsync()
    {
        string stampInDistributedCache;
        if (StoreCache.LastCheckTime.HasValue && DateTime.Now.Subtract(StoreCache.LastCheckTime.Value).TotalSeconds < 30.0)
        {
            stampInDistributedCache = null;
        }
        else
        {
            stampInDistributedCache = await GetOrSetStampInDistributedCache();
            if (stampInDistributedCache == StoreCache.CacheStamp)
            {
                StoreCache.LastCheckTime = new DateTime?(DateTime.Now);
                stampInDistributedCache = null;
            }
            else
            {
                await UpdateInMemoryStoreCache();
                StoreCache.CacheStamp = stampInDistributedCache;
                StoreCache.LastCheckTime = new DateTime?(DateTime.Now);
                stampInDistributedCache = null;
            }
        }
    }

    protected virtual async Task UpdateInMemoryStoreCache()
    {
        await StoreCache.FillAsync(await TextTemplateRepository.GetListAsync());
    }

    protected virtual async Task<string> GetOrSetStampInDistributedCache()
    {
        string cacheKey = GetCommonStampCacheKey();
        string stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
        if (stampInDistributedCache != null)
        {
            return stampInDistributedCache;
        }

        IAbpDistributedLockHandle commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(2.0));

        try
        {
            if (commonLockHandle == null)
            {
                throw new AbpException("Could not acquire distributed lock for template definition common stamp check!");
            }

            stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
            if (stampInDistributedCache == null)
            {
                stampInDistributedCache = Guid.NewGuid().ToString();
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = new TimeSpan?(TimeSpan.FromDays(30.0))
                };
                await DistributedCache.SetStringAsync(cacheKey, stampInDistributedCache, options);
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (commonLockHandle != null)
            {
                await commonLockHandle.DisposeAsync();
            }
        }

        return stampInDistributedCache;
    }

    protected virtual string GetCommonStampCacheKey() => CacheOptions.KeyPrefix + "_AbpInMemoryTextTemplateCacheStamp";

    protected virtual string GetCommonDistributedLockKey() => CacheOptions.KeyPrefix + "_Common_AbpTextTemplateUpdateLock";
}
