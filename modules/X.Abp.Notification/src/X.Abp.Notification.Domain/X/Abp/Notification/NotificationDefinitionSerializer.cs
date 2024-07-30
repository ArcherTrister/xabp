// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;

namespace X.Abp.Notification;

public class NotificationDefinitionSerializer : INotificationDefinitionSerializer, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }

    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    public NotificationDefinitionSerializer(IGuidGenerator guidGenerator, ILocalizableStringSerializer localizableStringSerializer)
    {
        GuidGenerator = guidGenerator;
        LocalizableStringSerializer = localizableStringSerializer;
    }

    public async Task<(NotificationGroupDefinitionRecord[] NotificationGroups, NotificationDefinitionRecord[] Notifications)> SerializeAsync(IEnumerable<NotificationGroupDefinition> notificationGroups)
    {
        var notificationGroupRecords = new List<NotificationGroupDefinitionRecord>();
        var notificationRecords = new List<NotificationDefinitionRecord>();

        foreach (var notificationGroup in notificationGroups)
        {
            notificationGroupRecords.Add(await SerializeAsync(notificationGroup));

            foreach (var notification in notificationGroup.GetNotificationsWithChildren())
            {
                notificationRecords.Add(await SerializeAsync(notification, notificationGroup));
            }
        }

        return (notificationGroupRecords.ToArray(), notificationRecords.ToArray());
    }

    public Task<NotificationGroupDefinitionRecord> SerializeAsync(NotificationGroupDefinition notificationGroup)
    {
        using (CultureHelper.Use(CultureInfo.InvariantCulture))
        {
            var notificationGroupRecord = new NotificationGroupDefinitionRecord(
                GuidGenerator.Create(),
                notificationGroup.Name,
                LocalizableStringSerializer.Serialize(notificationGroup.DisplayName));

            foreach (var property in notificationGroup.Properties)
            {
                notificationGroupRecord.SetProperty(property.Key, property.Value);
            }

            return Task.FromResult(notificationGroupRecord);
        }
    }

    public Task<NotificationDefinitionRecord> SerializeAsync(NotificationDefinition notification, NotificationGroupDefinition notificationGroup)
    {
        using (CultureHelper.Use(CultureInfo.InvariantCulture))
        {
            var notificationRecord = new NotificationDefinitionRecord(
                GuidGenerator.Create(),
                notificationGroup?.Name,
                notification.Name,
                notification.Parent?.Name,
                LocalizableStringSerializer.Serialize(notification.DisplayName),
                LocalizableStringSerializer.Serialize(notification.Description),
                notification.IsVisibleToClients,
                notification.IsAvailableToHost);

            foreach (var property in notification.Properties)
            {
                notificationRecord.SetProperty(property.Key, property.Value);
            }

            return Task.FromResult(notificationRecord);
        }
    }
}
