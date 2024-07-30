// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification
{
    public class NotificationInfoConsts
    {
        /// <summary>
        /// Maximum length of NotificationName property.
        /// Value: 96.
        /// </summary>
        public const int MaxNotificationNameLength = 96;

        /// <summary>
        /// Maximum length of Data property.
        /// Value: 1048576 (1 MB).
        /// </summary>
        public const int MaxDataLength = 1024 * 1024;

        /// <summary>
        /// Maximum length of DataTypeName property.
        /// Value: 512.
        /// </summary>
        public const int MaxDataTypeNameLength = 512;

        /// <summary>
        /// Maximum length of EntityTypeName property.
        /// Value: 250.
        /// </summary>
        public const int MaxEntityTypeNameLength = 250;

        /// <summary>
        /// Maximum length of EntityTypeAssemblyQualifiedName property.
        /// Value: 512.
        /// </summary>
        public const int MaxEntityTypeAssemblyQualifiedNameLength = 512;

        /// <summary>
        /// Maximum length of EntityId property.
        /// Value: 96.
        /// </summary>
        public const int MaxEntityIdLength = 96;

        /// <summary>
        /// Maximum length of UserIds property.
        /// Value: 1048576 (1 MB).
        /// </summary>
        public const int MaxUserIdsLength = 1024 * 1024;

        /// <summary>
        /// Maximum length of TenantIds property.
        /// Value: 131072 (128 KB).
        /// </summary>
        public const int MaxTenantIdsLength = 128 * 1024;

        /// <summary>
        /// Maximum length of TargetNotifiers property.
        /// Value: 1024 (1 KB).
        /// </summary>
        public const int MaxTargetNotifiersLength = 1024;
    }
}
