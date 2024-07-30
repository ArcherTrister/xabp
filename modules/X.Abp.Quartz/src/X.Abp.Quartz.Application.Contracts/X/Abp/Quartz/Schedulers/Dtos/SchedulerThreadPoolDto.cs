// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;
using Quartz.Util;

namespace X.Abp.Quartz.Schedulers.Dtos;

public class SchedulerThreadPoolDto
{
    public SchedulerThreadPoolDto(SchedulerMetaData metaData)
    {
        Type = metaData.ThreadPoolType.AssemblyQualifiedNameWithoutVersion();
        Size = metaData.ThreadPoolSize;
    }

    public string Type { get; private set; }

    public int Size { get; private set; }
}
