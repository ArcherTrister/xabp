// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Quartz;
using Quartz.Util;

namespace X.Abp.Quartz.Jobs.Dtos;

public class JobDetailDto
{
    public JobDetailDto(IJobDetail jobDetail)
    {
        Durable = jobDetail.Durable;
        ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
        Description = jobDetail.Description;
        JobType = jobDetail.JobType.AssemblyQualifiedNameWithoutVersion();
        Name = jobDetail.Key.Name;
        Group = jobDetail.Key.Group;
        PersistJobDataAfterExecution = jobDetail.PersistJobDataAfterExecution;
        RequestsRecovery = jobDetail.RequestsRecovery;
    }

    public string Name { get; set; }

    public string Group { get; set; }

    public string JobType { get; set; }

    public string Description { get; set; }

    public bool Durable { get; set; }

    public bool RequestsRecovery { get; set; }

    public bool PersistJobDataAfterExecution { get; set; }

    public bool ConcurrentExecutionDisallowed { get; set; }
}
