// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace X.Abp.AuditLogging.Dtos;

public class AuditLogActionDto : ExtensibleEntityDto<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid AuditLogId { get; set; }

    public string ServiceName { get; set; }

    public string MethodName { get; set; }

    public string Parameters { get; set; }

    public DateTime ExecutionTime { get; set; }

    public int ExecutionDuration { get; set; }
}
