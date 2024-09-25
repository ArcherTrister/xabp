// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.AuditLogging
{
    public static class AuditLogSettingNames
    {
        public const string GroupName = "Abp.AuditLogging";
        public const string IsPeriodicDeleterEnabled = GroupName + ".IsPeriodicDeleterEnabled";
        public const string IsExpiredDeleterEnabled = GroupName + ".IsExpiredDeleterEnabled";
        public const string ExpiredDeleterPeriod = GroupName + ".ExpiredDeleterPeriod";
    }
}
