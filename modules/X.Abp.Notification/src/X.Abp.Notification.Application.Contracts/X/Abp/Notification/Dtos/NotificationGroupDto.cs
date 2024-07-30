// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.Notification.Dtos;
public class NotificationGroupDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public List<NotificationDto> Notifications { get; set; }

    public string GetNormalizedGroupName()
    {
        return Name.Replace(".", "_");
    }
}
