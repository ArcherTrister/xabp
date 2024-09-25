// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
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
using Volo.Abp.Json.SystemTextJson.Modifiers;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace X.Abp.Notification;

public class StaticNotificationSaver : IStaticNotificationSaver, ITransientDependency
{
  protected IStaticNotificationDefinitionStore StaticStore { get; }

  protected INotificationGroupDefinitionRecordRepository NotificationGroupRepository { get; }

  protected INotificationDefinitionRecordRepository NotificationRepository { get; }

  protected INotificationDefinitionSerializer NotificationSerializer { get; }

  protected IDistributedCache Cache { get; }

  protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

  protected IAbpDistributedLock DistributedLock { get; }

  protected AbpNotificationOptions NotificationOptions { get; }

  protected ICancellationTokenProvider CancellationTokenProvider { get; }

  protected AbpDistributedCacheOptions CacheOptions { get; }

  protected IUnitOfWorkManager UnitOfWorkManager { get; }

  public StaticNotificationSaver(
      IStaticNotificationDefinitionStore staticStore,
      INotificationGroupDefinitionRecordRepository notificationGroupRepository,
      INotificationDefinitionRecordRepository notificationRepository,
      INotificationDefinitionSerializer notificationSerializer,
      IDistributedCache cache,
      IOptions<AbpDistributedCacheOptions> cacheOptions,
      IApplicationInfoAccessor applicationInfoAccessor,
      IAbpDistributedLock distributedLock,
      IOptions<AbpNotificationOptions> notificationManagementOptions,
      ICancellationTokenProvider cancellationTokenProvider,
      IUnitOfWorkManager unitOfWorkManager)
  {
    StaticStore = staticStore;
    NotificationGroupRepository = notificationGroupRepository;
    NotificationRepository = notificationRepository;
    NotificationSerializer = notificationSerializer;
    Cache = cache;
    ApplicationInfoAccessor = applicationInfoAccessor;
    DistributedLock = distributedLock;
    CancellationTokenProvider = cancellationTokenProvider;
    UnitOfWorkManager = unitOfWorkManager;
    NotificationOptions = notificationManagementOptions.Value;
    CacheOptions = cacheOptions.Value;
  }

  public virtual async Task SaveAsync()
  {
    await using var applicationLockHandle = await DistributedLock.TryAcquireAsync(
        GetApplicationDistributedLockKey());

    if (applicationLockHandle == null)
    {
      /* Another application instance is already doing it */
      return;
    }

    /* NOTE: This can be further optimized by using 4 cache values for:
     * Groups, notifications, deleted groups and deleted notifications.
     * But the code would be more complex. This is enough for now.
     */

    var cacheKey = GetApplicationHashCacheKey();
    var cachedHash = await Cache.GetStringAsync(cacheKey, CancellationTokenProvider.Token);

    var (notificationGroupRecords, notificationRecords) = await NotificationSerializer.SerializeAsync(
        await StaticStore.GetGroupsAsync());

    var currentHash = CalculateHash(
        notificationGroupRecords,
        notificationRecords,
        NotificationOptions.DeletedNotificationGroups,
        NotificationOptions.DeletedNotifications);

    if (cachedHash == currentHash)
    {
      return;
    }

    await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(
                     GetCommonDistributedLockKey(),
                     TimeSpan.FromMinutes(5)))
    {
      if (commonLockHandle == null)
      {
        /* It will re-try */
        throw new AbpException("Could not acquire distributed lock for saving static notifications!");
      }

      using (var unitOfWork = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
      {
        try
        {
          var hasChangesInGroups = await UpdateChangedNotificationGroupsAsync(notificationGroupRecords);
          var hasChangesInNotifications = await UpdateChangedNotificationsAsync(notificationRecords);

          if (hasChangesInGroups || hasChangesInNotifications)
          {
            await Cache.SetStringAsync(
                GetCommonStampCacheKey(),
                Guid.NewGuid().ToString(),
                new DistributedCacheEntryOptions
                {
                  SlidingExpiration = TimeSpan.FromDays(30) // TODO: Make it configurable?
                },
                CancellationTokenProvider.Token);
          }
        }
        catch
        {
          try
          {
            await unitOfWork.RollbackAsync();
          }
          catch
          {
            /* ignored */
          }

          throw;
        }

        await unitOfWork.CompleteAsync();
      }
    }

    await Cache.SetStringAsync(
        cacheKey,
        currentHash,
        new DistributedCacheEntryOptions
        {
          SlidingExpiration = TimeSpan.FromDays(30) // TODO: Make it configurable?
        },
        CancellationTokenProvider.Token);
  }

  private async Task<bool> UpdateChangedNotificationGroupsAsync(
      IEnumerable<NotificationGroupDefinitionRecord> notificationGroupRecords)
  {
    var newRecords = new List<NotificationGroupDefinitionRecord>();
    var changedRecords = new List<NotificationGroupDefinitionRecord>();

    var notificationGroupRecordsInDatabase = (await NotificationGroupRepository.GetListAsync())
        .ToDictionary(x => x.Name);

    foreach (var notificationGroupRecord in notificationGroupRecords)
    {
      var notificationGroupRecordInDatabase = notificationGroupRecordsInDatabase.GetOrDefault(notificationGroupRecord.Name);
      if (notificationGroupRecordInDatabase == null)
      {
        /* New group */
        newRecords.Add(notificationGroupRecord);
        continue;
      }

      if (notificationGroupRecord.HasSameData(notificationGroupRecordInDatabase))
      {
        /* Not changed */
        continue;
      }

      /* Changed */
      notificationGroupRecordInDatabase.Patch(notificationGroupRecord);
      changedRecords.Add(notificationGroupRecordInDatabase);
    }

    /* Deleted */
    var deletedRecords = NotificationOptions.DeletedNotificationGroups.Any()
        ? notificationGroupRecordsInDatabase.Values
            .Where(x => NotificationOptions.DeletedNotificationGroups.Contains(x.Name))
            .ToArray()
        : Array.Empty<NotificationGroupDefinitionRecord>();

    if (newRecords.Any())
    {
      await NotificationGroupRepository.InsertManyAsync(newRecords);
    }

    if (changedRecords.Any())
    {
      await NotificationGroupRepository.UpdateManyAsync(changedRecords);
    }

    if (deletedRecords.Any())
    {
      await NotificationGroupRepository.DeleteManyAsync(deletedRecords);
    }

    return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
  }

  private async Task<bool> UpdateChangedNotificationsAsync(
      IEnumerable<NotificationDefinitionRecord> notificationRecords)
  {
    var newRecords = new List<NotificationDefinitionRecord>();
    var changedRecords = new List<NotificationDefinitionRecord>();

    var notificationRecordsInDatabase = (await NotificationRepository.GetListAsync())
        .ToDictionary(x => x.Name);

    foreach (var notificationRecord in notificationRecords)
    {
      var notificationRecordInDatabase = notificationRecordsInDatabase.GetOrDefault(notificationRecord.Name);
      if (notificationRecordInDatabase == null)
      {
        /* New group */
        newRecords.Add(notificationRecord);
        continue;
      }

      if (notificationRecord.HasSameData(notificationRecordInDatabase))
      {
        /* Not changed */
        continue;
      }

      /* Changed */
      notificationRecordInDatabase.Patch(notificationRecord);
      changedRecords.Add(notificationRecordInDatabase);
    }

    /* Deleted */
    var deletedRecords = new List<NotificationDefinitionRecord>();

    if (NotificationOptions.DeletedNotifications.Any())
    {
      deletedRecords.AddRange(
          notificationRecordsInDatabase.Values
              .Where(x => NotificationOptions.DeletedNotifications.Contains(x.Name)));
    }

    if (NotificationOptions.DeletedNotificationGroups.Any())
    {
      deletedRecords.AddIfNotContains(
          notificationRecordsInDatabase.Values
              .Where(x => NotificationOptions.DeletedNotificationGroups.Contains(x.GroupName)));
    }

    if (newRecords.Any())
    {
      await NotificationRepository.InsertManyAsync(newRecords);
    }

    if (changedRecords.Any())
    {
      await NotificationRepository.UpdateManyAsync(changedRecords);
    }

    if (deletedRecords.Any())
    {
      await NotificationRepository.DeleteManyAsync(deletedRecords);
    }

    return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
  }

  private string GetApplicationDistributedLockKey()
  {
    return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpNotificationUpdateLock";
  }

  private string GetCommonDistributedLockKey()
  {
    return $"{CacheOptions.KeyPrefix}_Common_AbpNotificationUpdateLock";
  }

  private string GetApplicationHashCacheKey()
  {
    return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpNotificationsHash";
  }

  private string GetCommonStampCacheKey()
  {
    return $"{CacheOptions.KeyPrefix}_AbpInMemoryNotificationCacheStamp";
  }

  private static string CalculateHash(
      NotificationGroupDefinitionRecord[] notificationGroupRecords,
      NotificationDefinitionRecord[] notificationRecords,
      IEnumerable<string> deletedNotificationGroups,
      IEnumerable<string> deletedNotifications)
  {
    var jsonSerializerOptions = new JsonSerializerOptions
    {
      TypeInfoResolver = new DefaultJsonTypeInfoResolver
      {
        Modifiers =
                {
                    new AbpIgnorePropertiesModifiers<NotificationGroupDefinitionRecord, Guid>().CreateModifyAction(x => x.Id),
                    new AbpIgnorePropertiesModifiers<NotificationDefinitionRecord, Guid>().CreateModifyAction(x => x.Id)
                }
      }
    };

    var stringBuilder = new StringBuilder();

    stringBuilder.Append("NotificationGroupRecords:");
    stringBuilder.AppendLine(JsonSerializer.Serialize(notificationGroupRecords, jsonSerializerOptions));

    stringBuilder.Append("NotificationRecords:");
    stringBuilder.AppendLine(JsonSerializer.Serialize(notificationRecords, jsonSerializerOptions));

    stringBuilder.Append("DeletedNotificationGroups:");
    stringBuilder.AppendLine(deletedNotificationGroups.JoinAsString(","));

    stringBuilder.Append("DeletedNotification:");
    stringBuilder.Append(deletedNotifications.JoinAsString(","));

    return stringBuilder
        .ToString()
        .ToMd5();
  }
}
