// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace X.Abp.Notification.EntityFrameworkCore;

public static class NotificationDbContextModelCreatingExtensions
{
    public static void ConfigureNotification(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<NotificationGroupDefinitionRecord>(b =>
        {
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "NotificationGroups", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name).HasMaxLength(NotificationGroupDefinitionRecordConsts.MaxNameLength).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(NotificationGroupDefinitionRecordConsts.MaxDisplayNameLength).IsRequired();

            b.HasIndex(x => new { x.Name }).IsUnique();

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<NotificationDefinitionRecord>(b =>
        {
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "Notifications", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.GroupName).HasMaxLength(NotificationGroupDefinitionRecordConsts.MaxNameLength).IsRequired();
            b.Property(x => x.Name).HasMaxLength(NotificationDefinitionRecordConsts.MaxNameLength).IsRequired();
            b.Property(x => x.ParentName).HasMaxLength(NotificationDefinitionRecordConsts.MaxNameLength);
            b.Property(x => x.DisplayName).HasMaxLength(NotificationDefinitionRecordConsts.MaxDisplayNameLength).IsRequired();
            b.Property(x => x.Description).HasMaxLength(NotificationDefinitionRecordConsts.MaxDescriptionLength);

            b.HasIndex(x => new { x.Name }).IsUnique();
            b.HasIndex(x => new { x.GroupName });

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<NotificationInfo>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "NotificationInfos", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            // Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationInfoConsts.MaxNotificationNameLength);
            b.Property(q => q.Data).HasMaxLength(NotificationInfoConsts.MaxDataLength);
            b.Property(q => q.DataTypeName).HasMaxLength(NotificationInfoConsts.MaxDataTypeNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationInfoConsts.MaxEntityIdLength);
            b.Property(q => q.UserIds).HasMaxLength(NotificationInfoConsts.MaxUserIdsLength);
            b.Property(q => q.ExcludedUserIds).HasMaxLength(NotificationInfoConsts.MaxUserIdsLength);
            b.Property(q => q.TenantIds).HasMaxLength(NotificationInfoConsts.MaxTenantIdsLength);
            b.Property(q => q.TargetNotifiers).HasMaxLength(NotificationInfoConsts.MaxTargetNotifiersLength);

            // Indexes
            b.HasIndex(q => q.NotificationName);
        });

        builder.Entity<UserNotificationSubscription>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "NotificationSubscriptions", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            // Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationInfoConsts.MaxNotificationNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationInfoConsts.MaxEntityIdLength);
            b.Property(q => q.TargetNotifiers).HasMaxLength(NotificationInfoConsts.MaxTargetNotifiersLength);
        });

        builder.Entity<PublishedNotification>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "PublishedNotifications", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            // Properties
            b.Property(q => q.NotificationName).IsRequired().HasMaxLength(NotificationInfoConsts.MaxNotificationNameLength);
            b.Property(q => q.Data).HasMaxLength(NotificationInfoConsts.MaxDataLength);
            b.Property(q => q.DataTypeName).HasMaxLength(NotificationInfoConsts.MaxDataTypeNameLength);
            b.Property(q => q.EntityTypeName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeNameLength);
            b.Property(q => q.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationInfoConsts.MaxEntityTypeAssemblyQualifiedNameLength);
            b.Property(q => q.EntityId).HasMaxLength(NotificationInfoConsts.MaxEntityIdLength);
        });

        builder.Entity<UserNotification>(b =>
        {
            // Configure table & schema name
            b.ToTable(AbpNotificationDbProperties.DbTablePrefix + "UserNotifications", AbpNotificationDbProperties.DbSchema);

            b.ConfigureByConvention();

            // Properties
            b.Property(q => q.TargetNotifiers).HasMaxLength(NotificationInfoConsts.MaxTargetNotifiersLength);
        });

        builder.TryConfigureObjectExtensions<NotificationDbContext>();
    }
}
