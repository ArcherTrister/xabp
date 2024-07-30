// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace X.Abp.AuditLogging.Dtos;

public class EntityPropertyChangeDto : EntityDto<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid EntityChangeId { get; set; }

    public string NewValue { get; set; }

    public string OriginalValue { get; set; }

    public string PropertyName { get; set; }

    public string PropertyTypeFullName { get; set; }
}
