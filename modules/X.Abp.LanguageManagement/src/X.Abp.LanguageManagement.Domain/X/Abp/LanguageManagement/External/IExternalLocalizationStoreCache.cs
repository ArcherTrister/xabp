// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.LanguageManagement.External
{
    public interface IExternalLocalizationStoreCache
    {
        Task<ExternalLocalizationStoreCache.AllLocalizationResourcesCacheItem> GetAllResourcesCacheItemAsync(
            Func<Task<ExternalLocalizationStoreCache.AllLocalizationResourcesCacheItem>> factory
        );

        ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem GetResourceCacheItem(
            string resourceName,
            Func<ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem> factory
        );

        Task<ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem> GetResourceCacheItemAsync(
            string resourceName,
            Func<Task<ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem>> factory
        );

        ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem CreateResourceCacheItem(
            LocalizationResourceRecord resourceRecord
        );

        Task InvalidateAsync(IEnumerable<string> changedResourceNames);
    }
}
