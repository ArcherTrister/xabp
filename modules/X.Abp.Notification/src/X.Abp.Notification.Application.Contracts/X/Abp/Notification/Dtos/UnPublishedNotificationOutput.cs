// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Notification.Dtos
{
    public class UnPublishedNotificationOutput
    {
        public Guid Id { get; set; }

        public string NotificationName { get; set; }

        public string Data { get; set; }

        public string DataTypeName { get; set; }

        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
