// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using AsyncKeyedLock;

using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;

namespace X.Abp.LanguageManagement.External
{
    public class ExternalLocalizationTextCache : ISingletonDependency, IExternalLocalizationTextCache
    {
        private readonly AsyncKeyedLocker<string> asyncKeyedLocker = new AsyncKeyedLocker<string>(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        protected IDistributedCache<ExternalLocalizationTextCacheItem> TextsDistributedCache { get; }

        protected IDistributedCache<ExternalLocalizationTextCacheStampItem> StampsDistributedCache { get; }

        protected ConcurrentDictionary<string, MemoryCacheItem> MemoryCache { get; } = new ConcurrentDictionary<string, MemoryCacheItem>();

        protected IAbpDistributedLock DistributedLock { get; }

        public ExternalLocalizationTextCache(
            IDistributedCache<ExternalLocalizationTextCacheItem> textsDistributedCache,
            IDistributedCache<ExternalLocalizationTextCacheStampItem> stampsDistributedCache,
            IAbpDistributedLock distributedLock)
        {
            TextsDistributedCache = textsDistributedCache;
            StampsDistributedCache = stampsDistributedCache;
            DistributedLock = distributedLock;
        }

        public virtual Dictionary<string, string> TryGetTextsFromCache(string resourceName, string cultureName)
        {
            MemoryCacheItem memoryCacheItem = MemoryCache.GetOrDefault(GetTextsCacheKey(resourceName, cultureName));
            return memoryCacheItem != null && !IsShouldCheck(memoryCacheItem)
                ? memoryCacheItem.Texts
                : null;
        }

        public virtual async Task<Dictionary<string, string>> GetTextsAsync(
            string resourceName,
            string cultureName,
            Func<Task<Dictionary<string, string>>> factory)
        {
            string textsCacheKey = GetTextsCacheKey(resourceName, cultureName);
            MemoryCacheItem memoryCacheItem = MemoryCache.GetOrDefault(textsCacheKey);
            if (memoryCacheItem != null && !IsShouldCheck(memoryCacheItem))
            {
                return memoryCacheItem.Texts;
            }

            using (await asyncKeyedLocker.LockAsync(textsCacheKey))
            {
                memoryCacheItem = MemoryCache.GetOrDefault(textsCacheKey);
                if (memoryCacheItem != null && !IsShouldCheck(memoryCacheItem))
                {
                    return memoryCacheItem.Texts;
                }

                string cacheStamp = await GetCacheStampAsync(resourceName, cultureName);
                if (memoryCacheItem != null && memoryCacheItem.CacheStamp == cacheStamp)
                {
                    memoryCacheItem.LastCheckTime = DateTime.Now;
                    return memoryCacheItem.Texts;
                }

                ExternalLocalizationTextCacheItem textsCacheItem =
                    await TextsDistributedCache.GetAsync(textsCacheKey);
                if (textsCacheItem != null)
                {
                    MemoryCache[textsCacheKey] = new MemoryCacheItem(
                        textsCacheItem.Dictionary,
                        cacheStamp);
                    return textsCacheItem.Dictionary;
                }

                IAbpDistributedLockHandle textsLockHandle = await DistributedLock.TryAcquireAsync(textsCacheKey + "_TextsLock", TimeSpan.FromMinutes(1.0));
                try
                {
                    if (textsLockHandle == null)
                    {
                        throw new AbpException("Could not acquire distributed lock for getting localization items: " + textsCacheKey);
                    }

                    cacheStamp = await GetCacheStampAsync(resourceName, cultureName);
                    textsCacheItem = await TextsDistributedCache.GetAsync(textsCacheKey);
                    if (textsCacheItem != null)
                    {
                        MemoryCache[textsCacheKey] = new MemoryCacheItem(
                            textsCacheItem.Dictionary,
                            cacheStamp);
                        return textsCacheItem.Dictionary;
                    }
                    else
                    {
                        textsCacheItem = new ExternalLocalizationTextCacheItem(await factory());
                        await TextsDistributedCache.SetAsync(
                            textsCacheKey,
                            textsCacheItem,
                            CreateDistributedCacheEntryOptions());
                        ExternalLocalizationTextCacheStampItem cacheStampItem =
                            ExternalLocalizationTextCacheStampItem.Create();
                        await StampsDistributedCache.SetAsync(
                            GetStampCacheKey(resourceName, cultureName),
                            cacheStampItem);
                        MemoryCache[textsCacheKey] = new MemoryCacheItem(
                            textsCacheItem.Dictionary,
                            cacheStampItem.Stamp);
                        return textsCacheItem.Dictionary;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    return new Dictionary<string, string>();
                }
                finally
                {
                    if (textsLockHandle != null)
                    {
                        await textsLockHandle.DisposeAsync();
                    }
                }
            }
        }

        private static bool IsShouldCheck(MemoryCacheItem memoryCacheItem)
        {
            return DateTime.Now.Subtract(memoryCacheItem.LastCheckTime).TotalSeconds >= 30.0;
        }

        private async Task<string> GetCacheStampAsync(string resourceName, string cultureName)
        {
            string stampCacheKey = GetStampCacheKey(resourceName, cultureName);
            ExternalLocalizationTextCacheStampItem externalLocalizationTextCacheStampItem =
                await StampsDistributedCache.GetAsync(stampCacheKey);
            if (externalLocalizationTextCacheStampItem != null)
            {
                return externalLocalizationTextCacheStampItem.Stamp;
            }

            IAbpDistributedLockHandle stampLockHandle = await DistributedLock.TryAcquireAsync($"{stampCacheKey}_StampLock", TimeSpan.FromMinutes(1.0));

            try
            {
                if (stampLockHandle == null)
                {
                    throw new AbpException("Could not acquire distributed lock for updating localization stamp: " + stampCacheKey);
                }

                externalLocalizationTextCacheStampItem = await StampsDistributedCache.GetAsync(stampCacheKey);

                if (externalLocalizationTextCacheStampItem != null)
                {
                    return externalLocalizationTextCacheStampItem.Stamp;
                }
                else
                {
                    externalLocalizationTextCacheStampItem =
                        ExternalLocalizationTextCacheStampItem.Create();
                    await StampsDistributedCache.SetAsync(
                        stampCacheKey,
                        externalLocalizationTextCacheStampItem,
                        CreateDistributedCacheEntryOptions());

                    return externalLocalizationTextCacheStampItem.Stamp;
                }
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();

                return null;
            }
            finally
            {
                if (stampLockHandle != null)
                {
                    await stampLockHandle.DisposeAsync();
                }
            }
        }

        public virtual async Task InvalidateAsync(string resourceName, string cultureName)
        {
            string textsCacheKey = ExternalLocalizationTextCacheItem.GetCacheKey(
                resourceName,
                cultureName);

            IAbpDistributedLockHandle distributedLockHandle = await DistributedLock.TryAcquireAsync($"{textsCacheKey}_TextsLock", TimeSpan.FromSeconds(10.0));
            try
            {
                await TextsDistributedCache.RemoveAsync(textsCacheKey);

                await StampsDistributedCache.SetAsync(
                    ExternalLocalizationTextCacheStampItem.GetCacheKey(
                        resourceName,
                        cultureName),
                    ExternalLocalizationTextCacheStampItem.Create());
            }
            finally
            {
                if (distributedLockHandle != null)
                {
                    await distributedLockHandle.DisposeAsync();
                }
            }
        }

        protected virtual DistributedCacheEntryOptions CreateDistributedCacheEntryOptions() =>
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30.0)
            };

        protected virtual string GetTextsCacheKey(string resourceName, string cultureName) =>
            ExternalLocalizationTextCacheItem.GetCacheKey(resourceName, cultureName);

        protected virtual string GetStampCacheKey(string resourceName, string cultureName) =>
            ExternalLocalizationTextCacheStampItem.GetCacheKey(resourceName, cultureName);

        [IgnoreMultiTenancy]
        [CacheName("AbpExternalLocalizationTexts")]
        [Serializable]
        public class ExternalLocalizationTextCacheItem
        {
            public Dictionary<string, string> Dictionary { get; set; }

            public ExternalLocalizationTextCacheItem()
            {
            }

            public ExternalLocalizationTextCacheItem(Dictionary<string, string> dictionary)
            {
                Dictionary = Check.NotNull(dictionary, nameof(dictionary));
            }

            public static string GetCacheKey(string resourceName, string cultureName)
            {
                return $"{resourceName}:{cultureName}";
            }
        }

        [CacheName("AbpExternalLocalizationTextCacheStamps")]
        [IgnoreMultiTenancy]
        [Serializable]
        public class ExternalLocalizationTextCacheStampItem
        {
            public string Stamp { get; set; }

            public ExternalLocalizationTextCacheStampItem()
            {
            }

            public ExternalLocalizationTextCacheStampItem(string stamp) => Stamp = stamp;

            public static ExternalLocalizationTextCacheStampItem Create() =>
                new ExternalLocalizationTextCacheStampItem(Guid.NewGuid().ToString());

            public static string GetCacheKey(string resourceName, string cultureName)
            {
                return $"{resourceName}:{cultureName}";
            }
        }

        public class MemoryCacheItem
        {
            public Dictionary<string, string> Texts { get; }

            public string CacheStamp { get; }

            public DateTime LastCheckTime { get; set; }

            public MemoryCacheItem(Dictionary<string, string> texts, string cacheStamp)
            {
                Texts = texts;
                CacheStamp = cacheStamp;
                LastCheckTime = DateTime.Now;
            }
        }
    }
}
