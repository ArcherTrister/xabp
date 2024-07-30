// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Notification;

public class NotificationDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string GroupName { get; set; }

    public string Name { get; set; }

    public string ParentName { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool IsVisibleToClients { get; set; }

    public bool IsAvailableToHost { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public NotificationDefinitionRecord()
    {
        IsVisibleToClients = true;
        IsAvailableToHost = true;
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public NotificationDefinitionRecord(
        Guid id,
        string groupName,
        string name,
        string parentName,
        string displayName = null,
        string description = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true)
        : base(id)
    {
        GroupName = Check.NotNullOrWhiteSpace(groupName, nameof(groupName), NotificationDefinitionRecordConsts.MaxNameLength);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), NotificationDefinitionRecordConsts.MaxNameLength);
        ParentName = Check.Length(parentName, nameof(parentName), NotificationDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), NotificationDefinitionRecordConsts.MaxDisplayNameLength);

        Description = Check.Length(description, nameof(description), NotificationDefinitionRecordConsts.MaxDescriptionLength);

        IsVisibleToClients = isVisibleToClients;
        IsAvailableToHost = isAvailableToHost;

        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(NotificationDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            return false;
        }

        if (GroupName != otherRecord.GroupName)
        {
            return false;
        }

        if (ParentName != otherRecord.ParentName)
        {
            return false;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            return false;
        }

        if (Description != otherRecord.Description)
        {
            return false;
        }

        if (IsVisibleToClients != otherRecord.IsVisibleToClients)
        {
            return false;
        }

        if (IsAvailableToHost != otherRecord.IsAvailableToHost)
        {
            return false;
        }

        if (!this.HasSameExtraProperties(otherRecord))
        {
            return false;
        }

        return true;
    }

    public void Patch(NotificationDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            Name = otherRecord.Name;
        }

        if (GroupName != otherRecord.GroupName)
        {
            GroupName = otherRecord.GroupName;
        }

        if (ParentName != otherRecord.ParentName)
        {
            ParentName = otherRecord.ParentName;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            DisplayName = otherRecord.DisplayName;
        }

        if (Description != otherRecord.Description)
        {
            Description = otherRecord.Description;
        }

        if (IsVisibleToClients != otherRecord.IsVisibleToClients)
        {
            IsVisibleToClients = otherRecord.IsVisibleToClients;
        }

        if (IsAvailableToHost != otherRecord.IsAvailableToHost)
        {
            IsAvailableToHost = otherRecord.IsAvailableToHost;
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
