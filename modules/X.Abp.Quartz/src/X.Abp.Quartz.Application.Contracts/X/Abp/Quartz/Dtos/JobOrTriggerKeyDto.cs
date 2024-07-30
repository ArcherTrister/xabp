// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;

namespace X.Abp.Quartz.Dtos;

public class JobOrTriggerKeyDto
{
    public JobOrTriggerKeyDto(JobKey jobKey)
    {
        Name = jobKey.Name;
        Group = jobKey.Group;
    }

    public JobOrTriggerKeyDto(TriggerKey triggerKey)
    {
        Name = triggerKey.Name;
        Group = triggerKey.Group;
    }

    public string Name { get; private set; }

    public string Group { get; private set; }
}
