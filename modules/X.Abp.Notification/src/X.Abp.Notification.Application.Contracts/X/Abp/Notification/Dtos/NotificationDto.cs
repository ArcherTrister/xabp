// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification.Dtos;
public class NotificationDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public int Depth { get; set; }

    public string ParentName { get; set; }
}
