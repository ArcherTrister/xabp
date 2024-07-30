// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Quartz.Schedulers.Dtos;

public enum SchedulerStatus
{
    Unknown = 0,
    Running = 1,
    Standby = 2,
    Shutdown = 3,
}
