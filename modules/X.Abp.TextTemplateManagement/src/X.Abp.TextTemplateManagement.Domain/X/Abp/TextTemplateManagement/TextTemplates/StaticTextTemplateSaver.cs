// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Guids;
using Volo.Abp.Json.SystemTextJson.Modifiers;
using Volo.Abp.TextTemplating;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace X.Abp.TextTemplateManagement.TextTemplates;
public class StaticTextTemplateSaver : IStaticTextTemplateSaver, ITransientDependency
{
    protected IStaticTemplateDefinitionStore StaticStore { get; }

    protected ITextTemplateDefinitionRecordRepository TextTemplateRepository { get; }

    protected ITextTemplateDefinitionContentRecordRepository TextTemplateContentRepository { get; }

    protected ITextTemplateDefinitionSerializer TextTemplateSerializer { get; }

    protected IDistributedCache Cache { get; }

    protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

    protected IAbpDistributedLock DistributedLock { get; }

    protected AbpTextTemplatingOptions TemplateOptions { get; }

    protected ICancellationTokenProvider CancellationTokenProvider { get; }

    protected AbpDistributedCacheOptions CacheOptions { get; }

    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected IGuidGenerator GuidGenerator { get; }

    public StaticTextTemplateSaver(
      IStaticTemplateDefinitionStore staticStore,
      ITextTemplateDefinitionRecordRepository textTemplateRepository,
      ITextTemplateDefinitionContentRecordRepository textTemplateContentRepository,
      ITextTemplateDefinitionSerializer textTemplateSerializer,
      IDistributedCache cache,
      IOptions<AbpDistributedCacheOptions> cacheOptions,
      IApplicationInfoAccessor applicationInfoAccessor,
      IAbpDistributedLock distributedLock,
      IOptions<AbpTextTemplatingOptions> templateManagementOptions,
      ICancellationTokenProvider cancellationTokenProvider,
      IUnitOfWorkManager unitOfWorkManager,
      IGuidGenerator guidGenerator)
    {
        StaticStore = staticStore;
        TextTemplateRepository = textTemplateRepository;
        TextTemplateContentRepository = textTemplateContentRepository;
        TextTemplateSerializer = textTemplateSerializer;
        Cache = cache;
        ApplicationInfoAccessor = applicationInfoAccessor;
        DistributedLock = distributedLock;
        CancellationTokenProvider = cancellationTokenProvider;
        TemplateOptions = templateManagementOptions.Value;
        CacheOptions = cacheOptions.Value;
        UnitOfWorkManager = unitOfWorkManager;
        GuidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public async Task SaveAsync()
    {
        IAbpDistributedLockHandle applicationLockHandle = await DistributedLock.TryAcquireAsync(GetApplicationDistributedLockKey());

        string cacheKey;
        string cachedHash;
        Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>> templateRecords;
        string currentHash;
        try
        {
            if (applicationLockHandle != null)
            {
                cacheKey = GetApplicationHashCacheKey();
                cachedHash = await Cache.GetStringAsync(cacheKey, CancellationTokenProvider.Token);
                templateRecords = await TextTemplateSerializer.SerializeAsync(await StaticStore.GetAllAsync());

                currentHash = CalculateHash(templateRecords, TemplateOptions.DeletedTemplates);
                if (!(cachedHash == currentHash))
                {
                    IAbpDistributedLockHandle commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(5.0));
                    try
                    {
                        if (commonLockHandle == null)
                        {
                            throw new AbpException("Could not acquire distributed lock for saving static templates!");
                        }

                        using (IUnitOfWork unitOfWork = UnitOfWorkManager.Begin(true, true))
                        {
                            try
                            {
                                if (await UpdateChangedTemplatesAsync(templateRecords))
                                {
                                    string commonStampCacheKey = GetCommonStampCacheKey();
                                    await Cache.SetStringAsync(commonStampCacheKey,
                                        Guid.NewGuid().ToString(),
                                        new DistributedCacheEntryOptions
                                        {
                                            SlidingExpiration = new TimeSpan?(TimeSpan.FromDays(30.0))
                                        },
                                        CancellationTokenProvider.Token);
                                }
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
                    }
                    finally
                    {
                        await commonLockHandle.DisposeAsync();
                    }

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = new TimeSpan?(TimeSpan.FromDays(30.0))
                    };

                    await Cache.SetStringAsync(cacheKey, currentHash, options, CancellationTokenProvider.Token);
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

    private async Task<bool> UpdateChangedTemplatesAsync(Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>> templateRecords)
    {
        List<TextTemplateDefinitionRecord> newRecords = new List<TextTemplateDefinitionRecord>();
        List<TextTemplateDefinitionContentRecord> newContentRecords = new List<TextTemplateDefinitionContentRecord>();
        List<TextTemplateDefinitionRecord> changedRecords = new List<TextTemplateDefinitionRecord>();
        Dictionary<string, TextTemplateDefinitionRecord> templateRecordsInDatabase = (await TextTemplateRepository.GetListAsync()).ToDictionary(x => x.Name);
        foreach (KeyValuePair<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>> templateRecord in templateRecords)
        {
            TextTemplateDefinitionRecord templateRecordInDatabase = templateRecordsInDatabase.GetOrDefault(templateRecord.Key.Name);
            if (templateRecordInDatabase == null)
            {
                newRecords.Add(templateRecord.Key);
                newContentRecords.AddRange(templateRecord.Value);
            }
            else
            {
                await TextTemplateContentRepository.DeleteByDefinitionIdAsync(templateRecord.Key.Id);
                foreach (TextTemplateDefinitionContentRecord definitionContentRecord in templateRecord.Value)
                {
                    definitionContentRecord.DefinitionId = templateRecordInDatabase.Id;
                }

                newContentRecords.AddRange(templateRecord.Value);
                if (!templateRecord.Key.HasSameData(templateRecordInDatabase))
                {
                    templateRecordInDatabase.Patch(templateRecord.Key);
                    changedRecords.Add(templateRecordInDatabase);
                }
            }
        }

        List<TextTemplateDefinitionRecord> deletedRecords = new List<TextTemplateDefinitionRecord>();
        if (TemplateOptions.DeletedTemplates.Count != 0)
        {
            deletedRecords.AddRange(templateRecordsInDatabase.Values.Where(p => TemplateOptions.DeletedTemplates.Contains(p.Name)));
        }

        bool isChanged = false;
        if (newRecords.Count != 0)
        {
            await TextTemplateRepository.InsertManyAsync(newRecords);
            await TextTemplateContentRepository.InsertManyAsync(newContentRecords);
            isChanged = true;
        }

        if (changedRecords.Count != 0)
        {
            await TextTemplateRepository.UpdateManyAsync(changedRecords);
            isChanged = true;
        }

        if (deletedRecords.Count != 0)
        {
            await TextTemplateContentRepository.DeleteByDefinitionIdAsync(deletedRecords.Select(x => x.Id).ToArray());
            await TextTemplateRepository.DeleteManyAsync(deletedRecords, false);
            isChanged = true;
        }

        return isChanged;
    }

    private string GetApplicationDistributedLockKey() => CacheOptions.KeyPrefix + "_" + ApplicationInfoAccessor.ApplicationName + "_AbpTextTemplateUpdateLock";

    private string GetCommonDistributedLockKey() => CacheOptions.KeyPrefix + "_Common_AbpTextTemplateUpdateLock";

    private string GetApplicationHashCacheKey() => CacheOptions.KeyPrefix + "_" + ApplicationInfoAccessor.ApplicationName + "_AbpTextTemplatesHash";

    private string GetCommonStampCacheKey() => CacheOptions.KeyPrefix + "_AbpInMemoryTextTemplateCacheStamp";

    private static string CalculateHash(
      Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>> templateRecords,
      IEnumerable<string> deletedTemplates)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();

        DefaultJsonTypeInfoResolver typeInfoResolver = new DefaultJsonTypeInfoResolver();
        typeInfoResolver.Modifiers.Add(new AbpIgnorePropertiesModifiers<TextTemplateDefinitionRecord, Guid>().CreateModifyAction(x => x.Id));
        typeInfoResolver.Modifiers.Add(new AbpIgnorePropertiesModifiers<TextTemplateDefinitionContentRecord, Guid>().CreateModifyAction(x => x.Id));
        options.TypeInfoResolver = typeInfoResolver;

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("TemplateRecords:");
        stringBuilder.AppendLine(JsonSerializer.Serialize(templateRecords.Keys, options));
        stringBuilder.AppendLine(JsonSerializer.Serialize(templateRecords.Values, options));
        stringBuilder.Append("DeletedTemplate:");
        stringBuilder.Append(deletedTemplates.JoinAsString(","));
        return stringBuilder.ToString().ToMd5();
    }
}
