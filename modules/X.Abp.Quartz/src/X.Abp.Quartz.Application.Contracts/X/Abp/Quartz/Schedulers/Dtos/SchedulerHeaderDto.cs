// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;

namespace X.Abp.Quartz.Schedulers.Dtos;

public class SchedulerHeaderDto
{
    public SchedulerHeaderDto(IScheduler scheduler)
    {
        Name = scheduler.SchedulerName;
        SchedulerInstanceId = scheduler.SchedulerInstanceId;
        Status = TranslateStatus(scheduler);
    }

    public string Name { get; private set; }

    public string SchedulerInstanceId { get; private set; }

    public SchedulerStatus Status { get; private set; }

    internal static SchedulerStatus TranslateStatus(IScheduler scheduler)
    {
        return scheduler.IsShutdown
            ? SchedulerStatus.Shutdown
            : scheduler.InStandbyMode ? SchedulerStatus.Standby : scheduler.IsStarted ? SchedulerStatus.Running : SchedulerStatus.Unknown;
    }
}
