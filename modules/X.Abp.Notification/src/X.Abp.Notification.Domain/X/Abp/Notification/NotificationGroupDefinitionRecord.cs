// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Notification;

public class NotificationGroupDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public NotificationGroupDefinitionRecord()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public NotificationGroupDefinitionRecord(
        Guid id,
        string name,
        string displayName)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), NotificationGroupDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), NotificationGroupDefinitionRecordConsts.MaxDisplayNameLength);

        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(NotificationGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            return false;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            return false;
        }

        if (!this.HasSameExtraProperties(otherRecord))
        {
            return false;
        }

        return true;
    }

    public void Patch(NotificationGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            Name = otherRecord.Name;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            DisplayName = otherRecord.DisplayName;
        }

        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();

            foreach (var property in otherRecord.ExtraProperties)
            {
                ExtraProperties.Add(property.Key, property.Value);
            }
        }
    }
}
