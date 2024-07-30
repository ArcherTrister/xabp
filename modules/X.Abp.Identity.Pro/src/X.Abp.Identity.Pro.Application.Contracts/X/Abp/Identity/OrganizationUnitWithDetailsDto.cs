// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Identity;

public class OrganizationUnitWithDetailsDto : ExtensibleFullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public Guid? ParentId { get; set; }

    public string Code { get; set; }

    public string DisplayName { get; set; }

    public List<IdentityRoleDto> Roles { get; set; }

    public string ConcurrencyStamp { get; set; }
}
