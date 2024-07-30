// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.AuditLogging.Dtos;

public class EntityChangeDto : ExtensibleEntityDto<Guid>, IMultiTenant
{
    public Guid AuditLogId { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime ChangeTime { get; set; }

    public EntityChangeType ChangeType { get; set; }

    public string EntityId { get; set; }

    public string EntityTypeFullName { get; set; }

    public List<EntityPropertyChangeDto> PropertyChanges { get; set; }
}
