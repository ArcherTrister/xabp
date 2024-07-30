// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;

namespace X.Abp.Quartz.Schedulers.Dtos;

public class SchedulerStatisticsDto
{
    public SchedulerStatisticsDto(SchedulerMetaData metaData)
    {
        NumberOfJobsExecuted = metaData.NumberOfJobsExecuted;
    }

    public int NumberOfJobsExecuted { get; private set; }
}
