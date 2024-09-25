// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.Localization;
using Volo.Abp.Threading;

namespace X.Abp.LanguageManagement.External
{
    public class ExternalLocalizationResourceContributor : ILocalizationResourceContributor
    {
        public bool IsDynamic => false;

        protected LocalizationResourceBase Resource { get; private set; }

        protected ILocalizationTextRecordRepository LocalizationTextRecordRepository
        {
            get;
            private set;
        }

        protected ILocalizationResourceRecordRepository LocalizationResourceRecordRepository
        {
            get;
            private set;
        }

        protected IExternalLocalizationTextCache ExternalLocalizationTextCache { get; private set; }

        protected IExternalLocalizationStoreCache ExternalLocalizationStoreCache
        {
            get;
            private set;
        }

        public void Initialize(LocalizationResourceInitializationContext context)
        {
            Resource = context.Resource;
            LocalizationResourceRecordRepository =
                context.ServiceProvider.GetRequiredService<ILocalizationResourceRecordRepository>();
            LocalizationTextRecordRepository =
                context.ServiceProvider.GetRequiredService<ILocalizationTextRecordRepository>();
            ExternalLocalizationTextCache =
                context.ServiceProvider.GetRequiredService<IExternalLocalizationTextCache>();
            ExternalLocalizationStoreCache =
                context.ServiceProvider.GetRequiredService<IExternalLocalizationStoreCache>();
        }

        public LocalizedString GetOrNull(string cultureName, string name)
        {
            string text = GetTextsFromCache(cultureName).GetOrDefault(name);
            if (text == null)
            {
                return null;
            }

            return new LocalizedString(name, text);
        }

        public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary) =>
            InternalFill(dictionary, GetTextsFromCache(cultureName));

        public virtual async Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
        {
            InternalFill(dictionary, await GetTextsFromCacheAsync(cultureName));
        }

        private Dictionary<string, string> GetTextsFromCache(string cultureName)
        {
            return ExternalLocalizationTextCache.TryGetTextsFromCache(
                    Resource.ResourceName,
                    cultureName) ?? AsyncHelper.RunSync(() => GetTextsFromCacheAsync(cultureName));
        }

        private async Task<Dictionary<string, string>> GetTextsFromCacheAsync(string cultureName)
        {
            return await ExternalLocalizationTextCache.GetTextsAsync(
                Resource.ResourceName,
                cultureName,
                async () =>
                {
                    LocalizationTextRecord localizationTextRecord =
                        await LocalizationTextRecordRepository.FindAsync(
                            Resource.ResourceName,
                            cultureName);
                    return
                        localizationTextRecord == null
                        || localizationTextRecord.Value.IsNullOrWhiteSpace()
                        ? new Dictionary<string, string>()
                        : JsonSerializer.Deserialize<Dictionary<string, string>>(
                            localizationTextRecord.Value) ?? new Dictionary<string, string>();
                });
        }

        private static void InternalFill(Dictionary<string, LocalizedString> dictionary, Dictionary<string, string> texts)
        {
            foreach (KeyValuePair<string, string> keyValuePair in texts)
            {
                dictionary[keyValuePair.Key] = new LocalizedString(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public virtual async Task<IEnumerable<string>> GetSupportedCulturesAsync()
        {
            ExternalLocalizationStoreCache.LocalizationResourceRecordCacheItem localizationResourceRecordCacheItem =
                await ExternalLocalizationStoreCache.GetResourceCacheItemAsync(
                    Resource.ResourceName,
                    async () =>
                    {
                        LocalizationResourceRecord localizationResourceRecord =
                            await LocalizationResourceRecordRepository.FindAsync(
                                Resource.ResourceName);
                        return ExternalLocalizationStoreCache.CreateResourceCacheItem(
                            localizationResourceRecord);
                    });

            return localizationResourceRecordCacheItem.IsAvailable
                ? localizationResourceRecordCacheItem.SupportedCultures
                : Array.Empty<string>();
        }
    }
}
