// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Quartz;

using X.Abp.Quartz.Dtos;

namespace X.Abp.Quartz.Jobs.Dtos;

public class CurrentlyExecutingJobDto
{
    public CurrentlyExecutingJobDto(IJobExecutionContext context)
    {
        FireInstanceId = context.FireInstanceId;
        FireTime = context.FireTimeUtc;
        Trigger = new JobOrTriggerKeyDto(context.Trigger.Key);
        Job = new JobOrTriggerKeyDto(context.JobDetail.Key);
        JobRunTime = context.JobRunTime;
        RefireCount = context.RefireCount;

        Recovering = context.Recovering;
        if (context.Recovering)
        {
            RecoveringTrigger = new JobOrTriggerKeyDto(context.RecoveringTriggerKey);
        }
    }

    public string FireInstanceId { get; private set; }

    public DateTimeOffset? FireTime { get; private set; }

    public JobOrTriggerKeyDto Trigger { get; private set; }

    public JobOrTriggerKeyDto Job { get; private set; }

    public TimeSpan JobRunTime { get; private set; }

    public int RefireCount { get; private set; }

    public JobOrTriggerKeyDto RecoveringTrigger { get; private set; }

    public bool Recovering { get; private set; }
}
