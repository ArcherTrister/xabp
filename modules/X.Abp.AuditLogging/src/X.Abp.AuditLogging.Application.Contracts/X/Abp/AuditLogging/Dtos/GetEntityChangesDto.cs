// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace X.Abp.AuditLogging.Dtos;

public class GetEntityChangesDto : PagedAndSortedResultRequestDto
{
    public Guid? AuditLogId { get; set; }

    public EntityChangeType? EntityChangeType { get; set; }

    public string EntityId { get; set; }

    public string EntityTypeFullName { get; set; }

    public DateTime StartDate { get; set; } = DateTime.MinValue;

    public DateTime EndDate { get; set; } = DateTime.MaxValue;
}
