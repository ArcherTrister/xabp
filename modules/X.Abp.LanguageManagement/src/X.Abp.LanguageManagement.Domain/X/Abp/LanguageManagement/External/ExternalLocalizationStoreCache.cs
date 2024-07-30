// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace X.Abp.LanguageManagement.External
{
    public class ExternalLocalizationStoreCache : ISingletonDependency, IExternalLocalizationStoreCache
    {
        protected IDistributedCache<LocalizationResourceRecordCacheItem> ResourceCache { get; }

        protected IDistributedCache<AllLocalizationResourcesCacheItem> AllResourcesCache { get; }

        public ExternalLocalizationStoreCache(
            IDistributedCache<LocalizationResourceRecordCacheItem> resourceCache,
            IDistributedCache<AllLocalizationResourcesCacheItem> allResourcesCache)
        {
            ResourceCache = resourceCache;
            AllResourcesCache = allResourcesCache;
        }

        public virtual async Task<AllLocalizationResourcesCacheItem> GetAllResourcesCacheItemAsync(
            Func<Task<AllLocalizationResourcesCacheItem>> factory)
        {
            return await AllResourcesCache.GetOrAddAsync(AllLocalizationResourcesCacheItem.CacheKey, factory);
        }

        public virtual LocalizationResourceRecordCacheItem GetResourceCacheItem(
            string resourceName,
            Func<LocalizationResourceRecordCacheItem> factory)
        {
            return ResourceCache.GetOrAdd(resourceName, factory);
        }

        public virtual async Task<LocalizationResourceRecordCacheItem> GetResourceCacheItemAsync(
            string resourceName,
            Func<Task<LocalizationResourceRecordCacheItem>> factory)
        {
            return await ResourceCache.GetOrAddAsync(resourceName, factory);
        }

        public virtual async Task InvalidateAsync(IEnumerable<string> changedResourceNames)
        {
            await AllResourcesCache.RemoveAsync(AllLocalizationResourcesCacheItem.CacheKey);

            foreach (string changedResourceName in changedResourceNames)
            {
                await ResourceCache.RemoveAsync(changedResourceName);
            }
        }

        public virtual LocalizationResourceRecordCacheItem CreateResourceCacheItem(
            LocalizationResourceRecord resourceRecord)
        {
            if (resourceRecord == null)
            {
                return new LocalizationResourceRecordCacheItem() { IsAvailable = false };
            }

            return new LocalizationResourceRecordCacheItem()
            {
                Name = resourceRecord.Name,
                BaseResources = resourceRecord.GetBaseResources(),
                DefaultCulture = resourceRecord.DefaultCulture,
                SupportedCultures = resourceRecord.GetSupportedCultures(),
                IsAvailable = true
            };
        }

        [IgnoreMultiTenancy]
        [CacheName("AbpAllLocalizationResourceRecords")]
        public class AllLocalizationResourcesCacheItem
        {
            public const string CacheKey = "All";

            public LocalizationResourceRecordCacheItem[] Resources { get; set; }
        }

        [IgnoreMultiTenancy]
        [CacheName("AbpLocalizationResourceRecords")]
        public class LocalizationResourceRecordCacheItem
        {
            public virtual string Name { get; set; }

            public virtual string DefaultCulture { get; set; }

            public virtual string[] BaseResources { get; set; }

            public virtual string[] SupportedCultures { get; set; }

            public bool IsAvailable { get; set; }
        }
    }
}
