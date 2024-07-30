// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification.Dtos;
public class SubscribeInput
{
    public string NotificationName { get; set; }

    public EntityIdentifier EntityIdentifier { get; set; }

    public string[] TargetNotifiers { get; set; }
}
