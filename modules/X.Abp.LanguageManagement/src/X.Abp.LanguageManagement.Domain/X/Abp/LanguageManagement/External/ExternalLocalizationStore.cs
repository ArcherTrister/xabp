// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Localization.External;

namespace X.Abp.LanguageManagement.External
{
    [Dependency(ReplaceServices = true)]
    public class ExternalLocalizationStore : ITransientDependency, IExternalLocalizationStore
    {
        protected ILocalizationResourceRecordRepository LocalizationResourceRecordRepository { get; }

        protected AbpLocalizationOptions LocalizationOptions { get; }

        protected IExternalLocalizationStoreCache Cache { get; }

        public ExternalLocalizationStore(
            IOptions<AbpLocalizationOptions> localizationOptions,
            ILocalizationResourceRecordRepository localizationResourceRecordRepository,
            IExternalLocalizationStoreCache cache)
        {
            LocalizationResourceRecordRepository = localizationResourceRecordRepository;
            Cache = cache;
            LocalizationOptions = localizationOptions.Value;
        }

        public virtual LocalizationResourceBase GetResourceOrNull(string resourceName)
        {
            ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem resourceCacheItem =
                GetResourceCacheItem(resourceName);
            return !resourceCacheItem.IsAvailable
                ? null
                : CreateNonTypedLocalizationResource(resourceCacheItem);
        }

        public virtual async Task<LocalizationResourceBase> GetResourceOrNullAsync(
            string resourceName)
        {
            ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem resourceCacheItem =
                await GetResourceCacheItemAsync(resourceName);
            return resourceCacheItem.IsAvailable
                ? CreateNonTypedLocalizationResource(resourceCacheItem)
                : null;
        }

        public virtual async Task<string[]> GetResourceNamesAsync()
        {
            return (await GetResourcesAsync())
                .Where(l => !LocalizationOptions.Resources.ContainsKey(l.ResourceName))
                .Select(r => r.ResourceName)
                .ToArray();
        }

        public virtual async Task<LocalizationResourceBase[]> GetResourcesAsync()
        {
            return (await GetAllResourcesCacheItemAsync())
                .Resources.Select(CreateNonTypedLocalizationResource)
                .Cast<LocalizationResourceBase>()
                .ToArray();
        }

        protected virtual NonTypedLocalizationResource CreateNonTypedLocalizationResource(
            ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem resourceCacheItem)
        {
            NonTypedLocalizationResource localizationResource = new NonTypedLocalizationResource(
                resourceCacheItem.Name,
                resourceCacheItem.DefaultCulture);
            if (resourceCacheItem.BaseResources.Length != 0)
            {
                localizationResource.AddBaseResources(resourceCacheItem.BaseResources);
            }

            localizationResource.Contributors.Add(new ExternalLocalizationResourceContributor());
            return localizationResource;
        }

        protected virtual async Task<ExternalLocalizationStoreCache.AllLocalizationResourcesCacheItem> GetAllResourcesCacheItemAsync()
        {
            return await Cache.GetAllResourcesCacheItemAsync(
                    async () =>
                    {
                        var localizationResourceRecords = await LocalizationResourceRecordRepository.GetListAsync();

                        return new ExternalLocalizationStoreCache.AllLocalizationResourcesCacheItem()
                        {
                            Resources = localizationResourceRecords
                                .Where(l => !LocalizationOptions.Resources.ContainsKey(l.Name))
                                .Select(p => CreateResourceCacheItem(p))
                                .ToArray()
                        };
                    });
        }

        protected virtual ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem GetResourceCacheItem(
            string resourceName)
        {
            return Cache.GetResourceCacheItem(
                resourceName,
                () => CreateResourceCacheItem(LocalizationResourceRecordRepository.Find(resourceName)));
        }

        protected virtual async Task<ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem> GetResourceCacheItemAsync(
            string resourceName)
        {
            return await Cache.GetResourceCacheItemAsync(
                resourceName,
                async () =>
                {
                    LocalizationResourceRecord resourceRecord =
                        await LocalizationResourceRecordRepository.FindAsync(resourceName);
                    return CreateResourceCacheItem(resourceRecord);
                });
        }

        protected virtual ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem CreateResourceCacheItem(
            LocalizationResourceRecord resourceRecord)
        {
            return Cache.CreateResourceCacheItem(resourceRecord);
        }
    }
}
