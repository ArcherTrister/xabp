// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;
using Quartz.Util;

namespace X.Abp.Quartz.Schedulers.Dtos;

public class SchedulerJobStoreDto
{
    public SchedulerJobStoreDto(SchedulerMetaData metaData)
    {
        Type = metaData.JobStoreType.AssemblyQualifiedNameWithoutVersion();
        Clustered = metaData.JobStoreClustered;
        Persistent = metaData.JobStoreSupportsPersistence;
    }

    public string Type { get; private set; }

    public bool Clustered { get; private set; }

    public bool Persistent { get; private set; }
}
