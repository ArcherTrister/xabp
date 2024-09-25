// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Localization.External;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace X.Abp.LanguageManagement.External
{
    public class ExternalLocalizationSaver : ITransientDependency, IExternalLocalizationSaver
    {
        public ILogger<ExternalLocalizationSaver> Logger { get; set; }

        protected ILocalizationResourceRecordRepository LocalizationResourceRecordRepository { get; }

        protected ILocalizationTextRecordRepository LocalizationTextRecordRepository { get; }

        protected AbpLocalizationOptions LocalizationOptions { get; }

        protected IStringLocalizerFactory StringLocalizerFactory { get; }

        protected ILanguageProvider LanguageProvider { get; }

        protected IGuidGenerator GuidGenerator { get; }

        protected IExternalLocalizationTextCache ExternalLocalizationTextCache { get; }

        protected IExternalLocalizationStore ExternalLocalizationStore { get; }

        protected IExternalLocalizationStoreCache ExternalLocalizationStoreCache { get; }

        protected IDistributedCache<ResourceHashCacheItem> HashCache { get; }

        protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

        protected IAbpDistributedLock DistributedLock { get; }

        protected AbpDistributedCacheOptions CacheOptions { get; }

        protected IUnitOfWorkManager UnitOfWorkManager { get; }

        public ExternalLocalizationSaver(
            IOptions<AbpLocalizationOptions> localizationOptions,
            ILocalizationResourceRecordRepository localizationResourceRecordRepository,
            IStringLocalizerFactory stringLocalizerFactory,
            ILocalizationTextRecordRepository localizationTextRecordRepository,
            IGuidGenerator guidGenerator,
            ILanguageProvider languageProvider,
            IExternalLocalizationTextCache externalLocalizationTextCache,
            IExternalLocalizationStore externalLocalizationStore,
            IDistributedCache<ResourceHashCacheItem> hashCache,
            IApplicationInfoAccessor applicationInfoAccessor,
            IAbpDistributedLock distributedLock,
            IOptions<AbpDistributedCacheOptions> cacheOptions,
            IExternalLocalizationStoreCache externalLocalizationStoreCache,
            IUnitOfWorkManager unitOfWorkManager)
        {
            LocalizationResourceRecordRepository = localizationResourceRecordRepository;
            StringLocalizerFactory = stringLocalizerFactory;
            LocalizationTextRecordRepository = localizationTextRecordRepository;
            GuidGenerator = guidGenerator;
            LanguageProvider = languageProvider;
            ExternalLocalizationTextCache = externalLocalizationTextCache;
            ExternalLocalizationStore = externalLocalizationStore;
            HashCache = hashCache;
            ApplicationInfoAccessor = applicationInfoAccessor;
            DistributedLock = distributedLock;
            ExternalLocalizationStoreCache = externalLocalizationStoreCache;
            UnitOfWorkManager = unitOfWorkManager;
            CacheOptions = cacheOptions.Value;
            LocalizationOptions = localizationOptions.Value;
            Logger = NullLogger<ExternalLocalizationSaver>.Instance;
        }

        public virtual async Task SaveAsync()
        {
            Logger.LogDebug("Waiting to acquire the distributed lock for saving external localizations...");
            IAbpDistributedLockHandle applicationLockHandle = await DistributedLock.TryAcquireAsync(GetApplicationExternalLocalizationSavingDistributedLockKey());
            try
            {
                if (applicationLockHandle != null)
                {
                    Logger.LogInformation("Saving external localizations...");
                    IAbpDistributedLockHandle commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonExternalLocalizationSavingDistributedLockKey(), TimeSpan.FromMinutes(5.0));
                    try
                    {
                        if (commonLockHandle == null)
                        {
                            throw new AbpException("Could not acquire distributed lock for saving external localizations!");
                        }

                        using (IUnitOfWork unitOfWork = UnitOfWorkManager.Begin(true, true))
                        {
                            try
                            {
                                var context = await GetSaveContextAsync();
                                var localizationResourceArray = context.Resources;

                                for (int index = 0; index < localizationResourceArray.Length; ++index)
                                {
                                    await SaveLocalizationResourceAsync(context, localizationResourceArray[index]);
                                }

                                await AddOrUpdateAsync(context);

                                await InvalidateTextRecordsAsync(context);

                                await InvalidateResourceRecordsAsync(context);

                                await SetResourceHashAsync(context);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    await unitOfWork.RollbackAsync();
                                }
                                catch
                                {
                                }

                                throw;
                            }

                            await unitOfWork.CompleteAsync();
                        }

                        Logger.LogInformation("Completed to save external localizations.");
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
                }
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                if (applicationLockHandle != null)
                {
                    await applicationLockHandle.DisposeAsync();
                }
            }
        }

        private async Task<SaveContext> GetSaveContextAsync()
        {
            SaveContext context = new SaveContext(await LanguageProvider.GetLanguagesAsync(), GetResources());
            LocalizationResource[] localizationResourceArray = context.Resources;
            for (int i = 0; i < localizationResourceArray.Length; i++)
            {
                LocalizationResource resource = localizationResourceArray[i];
                IStringLocalizer stringLocalizer = StringLocalizerFactory.Create(resource.ResourceType);
                context.Localizers[resource.ResourceName] = stringLocalizer;
                context.SupportedCultures[resource.ResourceName] = (await stringLocalizer.GetSupportedCulturesAsync()).ToArray();
            }

            context.ResourcesHash = GetResourcesHash(context);

            context.ResourcesShouldBeUpdated = context.ResourcesHash != await GetResourcesHashByCacheAsync();

            return context;
        }

        private static string GetResourcesHash(SaveContext context)
        {
            return JsonSerializer
                .Serialize(
                    context
                        .Resources.Select(x => new
                        {
                            Name = x.ResourceName,
                            BaseResourceNames = x.BaseResourceNames.ToArray(),
                            DefaultCultureName = x.DefaultCultureName,
                            SupportedCultures = context.SupportedCultures[x.ResourceName]
                        })
                        .ToArray())
                .ToMd5();
        }

        private string GetApplicationExternalLocalizationSavingDistributedLockKey()
        {
            return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpExternalLocalizationSaving";
        }

        private string GetCommonExternalLocalizationSavingDistributedLockKey()
        {
            return $"{CacheOptions.KeyPrefix}_CommonAbpExternalLocalizationSaving";
        }

        private static string GetRecordTextLockKey(string resourceName, string cultureName)
        {
            return $"{resourceName}_{cultureName}";
        }

        private LocalizationResource[] GetResources()
        {
            return LocalizationOptions.Resources.Values.OfType<LocalizationResource>().ToArray();
        }

        private async Task SaveLocalizationResourceAsync(SaveContext context, LocalizationResource localizationResource)
        {
            IStringLocalizer stringLocalizer = StringLocalizerFactory.Create(localizationResource.ResourceType);
            var supportedCultures = (await stringLocalizer.GetSupportedCulturesAsync()).ToArray();

            if (context.ResourcesShouldBeUpdated)
            {
                await ResourcesShouldBeUpdatedCheck(context, localizationResource, supportedCultures);
            }

            foreach (string compatibleCulture in GetCompatibleCultures(context, localizationResource, supportedCultures))
            {
                await SavingRecordTextsAsync(context, localizationResource, compatibleCulture, stringLocalizer);
            }
        }

        private async Task ResourcesShouldBeUpdatedCheck(
            SaveContext context,
            LocalizationResource localizationResource,
            string[] supportedCultures)
        {
            LocalizationResourceRecord localizationResourceRecord = await LocalizationResourceRecordRepository.FindAsync(localizationResource.ResourceName);

            if (localizationResourceRecord == null)
            {
                context.NewResourceRecords.Add(new LocalizationResourceRecord(localizationResource, supportedCultures));
            }
            else
            {
                if (localizationResourceRecord.TryUpdate(localizationResource, supportedCultures))
                {
                    context.ChangedResourceRecords.Add(localizationResourceRecord);
                }
            }
        }

        private async Task InvalidateResourceRecordsAsync(SaveContext context)
        {
            if (
                context.NewResourceRecords.Count != 0
                || context.ChangedResourceRecords.Count != 0)
            {
                await ExternalLocalizationStoreCache.InvalidateAsync(
                    context.ChangedResourceRecords.Select(
                        x => x.Name));
            }
        }

        private async Task InvalidateTextRecordsAsync(SaveContext context)
        {
            foreach (
                var data in context
                    .NewTextRecords.Select(x => new
                    {
                        x.ResourceName,
                        x.CultureName
                    })
                    .Distinct()
                    .Union(
                        context
                            .ChangedTextRecords.Select(x => new
                            {
                                x.ResourceName,
                                x.CultureName
                            })
                            .Distinct()))
            {
                await ExternalLocalizationTextCache.InvalidateAsync(data.ResourceName, data.CultureName);
            }
        }

        private async Task AddOrUpdateAsync(SaveContext context)
        {
            if (context.NewResourceRecords.Count != 0)
            {
                await LocalizationResourceRecordRepository.InsertManyAsync(context.NewResourceRecords);
            }

            if (context.ChangedResourceRecords.Count != 0)
            {
                await LocalizationResourceRecordRepository.UpdateManyAsync(context.ChangedResourceRecords);
            }

            if (context.NewTextRecords.Count != 0)
            {
                await LocalizationTextRecordRepository.InsertManyAsync(context.NewTextRecords);
            }

            if (context.ChangedTextRecords.Count != 0)
            {
                await LocalizationTextRecordRepository.UpdateManyAsync(context.ChangedTextRecords);
            }
        }

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.All)
        };

        private async Task SavingRecordTextsAsync(
            SaveContext context,
            LocalizationResource localizationResource,
            string cultureName,
            IStringLocalizer stringLocalizer)
        {
            using (CultureHelper.Use(cultureName))
            {
                await using (var recordTextLockHandle =
                    await DistributedLock.TryAcquireAsync(
                        GetRecordTextLockKey(localizationResource.ResourceName, cultureName),
                        TimeSpan.FromMinutes(2.0)))
                {
                        if (recordTextLockHandle == null)
                        {
                            throw new AbpException("Could not acquire distributed lock for saving record text!");
                        }

                        var localizedStrings = await stringLocalizer.GetAllStringsAsync(false, false, false);

                        if (localizedStrings.Any())
                        {
                            var calculatedHash = GetCalculatedHash(localizedStrings);
                            if (
                                !(
                                    calculatedHash
                                    == await GetCalculatedHashAsync(localizationResource.ResourceName, cultureName)))
                            {
                                bool hasChange = false;
                                LocalizationTextRecord localizationTextRecord =
                                    await LocalizationTextRecordRepository.FindAsync(
                                        localizationResource.ResourceName,
                                        cultureName);
                                Dictionary<string, string> dictionary;
                                if (localizationTextRecord != null)
                                {
                                    dictionary =
                                        JsonSerializer.Deserialize<Dictionary<string, string>>(
                                            localizationTextRecord.Value) ?? new Dictionary<string, string>();
                                }
                                else
                                {
                                    dictionary = new Dictionary<string, string>();
                                    hasChange = true;
                                }

                                foreach (LocalizedString localizedString in localizedStrings)
                                {
                                    string value;
                                    if (dictionary.TryGetValue(localizedString.Name, out value))
                                    {
                                        if (value != localizedString.Value)
                                        {
                                            dictionary[localizedString.Name] = localizedString.Value;
                                            hasChange = true;
                                        }
                                    }
                                    else
                                    {
                                        dictionary.Add(localizedString.Name, localizedString.Value);
                                        hasChange = true;
                                    }
                                }

                                if (hasChange)
                                {
                                    string value = JsonSerializer.Serialize(
                                        dictionary,
                                        JsonSerializerOptions);
                                    if (localizationTextRecord == null)
                                    {
                                        context.NewTextRecords.Add(
                                            new LocalizationTextRecord(
                                                GuidGenerator.Create(),
                                                localizationResource.ResourceName,
                                                cultureName,
                                                value));
                                    }
                                    else
                                    {
                                        localizationTextRecord.Value = value;
                                        context.ChangedTextRecords.Add(localizationTextRecord);
                                    }
                            }

                                await SetCalculatedHashAsync(
                                    localizationResource.ResourceName,
                                    cultureName,
                                    calculatedHash);
                            }
                        }
                }
            }
        }

        private async Task<string> GetCalculatedHashAsync(string resourceName, string cultureName) =>
            (
                await HashCache.GetAsync(
                    GetCalculatedHashKey(resourceName, cultureName)))?.Hash;

        private async Task SetCalculatedHashAsync(string resourceName, string cultureName, string hash)
        {
            await HashCache.SetAsync(
                GetCalculatedHashKey(resourceName, cultureName),
                new ResourceHashCacheItem
                {
                    Hash = hash
                },
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(30.0)
                },
                true,
                true);
        }

        private async Task SetResourceHashAsync(SaveContext context)
        {
            if (!context.ResourcesShouldBeUpdated)
            {
                return;
            }

            await HashCache.SetAsync(
                GetResourcesHashKey(),
                new ResourceHashCacheItem
                {
                    Hash = context.ResourcesHash
                },
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(30.0)
                },
                true,
                true);
        }

        private string GetCalculatedHashKey(string resourceName, string cultureName)
        {
            return $"{ApplicationInfoAccessor.ApplicationName}_{resourceName}_{cultureName}_AbpTextsHash";
        }

        private static string GetCalculatedHash(IEnumerable<LocalizedString> texts)
        {
            return JsonSerializer
                .Serialize(
                    texts
                        .Select(
                            x => new NameValue(x.Name, x.Value))
                        .ToArray())
                .ToMd5();
        }

        private async Task<string> GetResourcesHashByCacheAsync()
        {
            return (await HashCache.GetAsync(GetResourcesHashKey()))?.Hash;
        }

        private string GetResourcesHashKey()
        {
            return $"{ApplicationInfoAccessor.ApplicationName}_AbpResourcesHash";
        }

        protected virtual IEnumerable<string> GetCompatibleCultures(
            SaveContext context,
            LocalizationResource resource,
            IEnumerable<string> supportedCultures)
        {
            return supportedCultures
                .Where(
                        cultureName =>
                            IsCompatibleCulture(
                                cultureName,
                                context.ApplicationLanguages))
                .Union([resource.DefaultCultureName]);
        }

        protected virtual bool IsCompatibleCulture(
            string resourceCultureName,
            IReadOnlyList<LanguageInfo> applicationLanguages)
        {
            foreach (LanguageInfo applicationLanguage in (IEnumerable<LanguageInfo>)applicationLanguages)
            {
                if (IsCompatibleCulture(resourceCultureName, applicationLanguage))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsCompatibleCulture(
            string resourceCultureName,
            LanguageInfo applicationLanguage)
        {
            return CultureHelper.IsCompatibleCulture(
                resourceCultureName,
                applicationLanguage.UiCultureName);
        }

        protected class SaveContext
        {
            public List<LocalizationResourceRecord> NewResourceRecords { get; } =
                new List<LocalizationResourceRecord>();

            public List<LocalizationResourceRecord> ChangedResourceRecords { get; } =
                new List<LocalizationResourceRecord>();

            public List<LocalizationTextRecord> NewTextRecords { get; } =
                new List<LocalizationTextRecord>();

            public List<LocalizationTextRecord> ChangedTextRecords { get; } =
                new List<LocalizationTextRecord>();

            public IReadOnlyList<LanguageInfo> ApplicationLanguages { get; }

            public LocalizationResource[] Resources { get; }

            public Dictionary<string, string[]> SupportedCultures { get; } =
                new Dictionary<string, string[]>();

            public Dictionary<string, IStringLocalizer> Localizers { get; } =
                new Dictionary<string, IStringLocalizer>();

            public bool ResourcesShouldBeUpdated { get; set; }

            public string ResourcesHash { get; set; }

            public SaveContext(
                IReadOnlyList<LanguageInfo> applicationLanguages,
                LocalizationResource[] resources)
            {
                ApplicationLanguages = applicationLanguages;
                Resources = resources;
            }
        }

        [CacheName("AbpExternalLocalizationSaving")]
        [IgnoreMultiTenancy]
        [Serializable]
        public class ResourceHashCacheItem
        {
            public string Hash { get; set; }
        }
    }
}
