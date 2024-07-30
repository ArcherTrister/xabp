// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;

namespace X.Abp.Quartz.Schedulers.Dtos;

public class SchedulerDto
{
    public SchedulerDto(IScheduler scheduler, SchedulerMetaData metaData)
    {
        Name = scheduler.SchedulerName;
        SchedulerInstanceId = scheduler.SchedulerInstanceId;
        Status = SchedulerHeaderDto.TranslateStatus(scheduler);

        ThreadPool = new SchedulerThreadPoolDto(metaData);
        JobStore = new SchedulerJobStoreDto(metaData);
        Statistics = new SchedulerStatisticsDto(metaData);
    }

    public string SchedulerInstanceId { get; }

    public string Name { get; }

    public SchedulerStatus Status { get; }

    public SchedulerThreadPoolDto ThreadPool { get; }

    public SchedulerJobStoreDto JobStore { get; }

    public SchedulerStatisticsDto Statistics { get; }
}
