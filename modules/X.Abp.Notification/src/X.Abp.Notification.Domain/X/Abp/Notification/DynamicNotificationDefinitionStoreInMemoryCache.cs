// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace X.Abp.Notification;

public class DynamicNotificationDefinitionStoreInMemoryCache :
    IDynamicNotificationDefinitionStoreInMemoryCache,
    ISingletonDependency
{
  public string CacheStamp { get; set; }

  protected IDictionary<string, NotificationGroupDefinition> NotificationGroupDefinitions { get; }

  protected IDictionary<string, NotificationDefinition> NotificationDefinitions { get; }

  protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

  public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);

  public DateTime? LastCheckTime { get; set; }

  public DynamicNotificationDefinitionStoreInMemoryCache(
      ILocalizableStringSerializer localizableStringSerializer)
  {
    LocalizableStringSerializer = localizableStringSerializer;

    NotificationGroupDefinitions = new Dictionary<string, NotificationGroupDefinition>();
    NotificationDefinitions = new Dictionary<string, NotificationDefinition>();
  }

  public virtual Task FillAsync(
      List<NotificationGroupDefinitionRecord> notificationGroupRecords,
      List<NotificationDefinitionRecord> notificationRecords)
  {
    NotificationGroupDefinitions.Clear();
    NotificationDefinitions.Clear();

    var context = new NotificationDefinitionContext();

    foreach (var notificationGroupRecord in notificationGroupRecords)
    {
      var notificationGroup = context.AddGroup(
          notificationGroupRecord.Name,
          LocalizableStringSerializer.Deserialize(notificationGroupRecord.DisplayName));

      NotificationGroupDefinitions[notificationGroup.Name] = notificationGroup;

      foreach (var property in notificationGroupRecord.ExtraProperties)
      {
        notificationGroup[property.Key] = property.Value;
      }

      var notificationRecordsInThisGroup = notificationRecords
          .Where(p => p.GroupName == notificationGroup.Name);

      foreach (var notificationRecord in notificationRecordsInThisGroup.Where(x => x.ParentName == null))
      {
        AddNotificationRecursively(notificationGroup, notificationRecord, notificationRecords);
      }
    }

    return Task.CompletedTask;
  }

  public NotificationDefinition GetNotificationOrNull(string name)
  {
    return NotificationDefinitions.GetOrDefault(name);
  }

  public IReadOnlyList<NotificationDefinition> GetNotifications()
  {
    return NotificationDefinitions.Values.ToList();
  }

  public IReadOnlyList<NotificationGroupDefinition> GetGroups()
  {
    return NotificationGroupDefinitions.Values.ToList();
  }

  private void AddNotificationRecursively(ICanCreateChildNotification notificationContainer,
      NotificationDefinitionRecord notificationRecord,
      List<NotificationDefinitionRecord> allNotificationRecords)
  {
    var notification = notificationContainer.CreateChildNotification(
        notificationRecord.Name,
        LocalizableStringSerializer.Deserialize(notificationRecord.DisplayName),
        LocalizableStringSerializer.Deserialize(notificationRecord.Description),
        notificationRecord.IsVisibleToClients,
        notificationRecord.IsAvailableToHost);

    NotificationDefinitions[notification.Name] = notification;

    foreach (var property in notificationRecord.ExtraProperties)
    {
      notification[property.Key] = property.Value;
    }

    foreach (var subNotification in allNotificationRecords.Where(p => p.ParentName == notificationRecord.Name))
    {
      AddNotificationRecursively(notification, subNotification, allNotificationRecords);
    }
  }
}
