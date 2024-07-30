// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Identity;

public class IdentityRoleDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public bool IsDefault { get; set; }

    public bool IsStatic { get; set; }

    public bool IsPublic { get; set; }

    public string ConcurrencyStamp { get; set; }
}
