// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Identity;

public class IdentityUserDto : ExtensibleAuditedEntityDto<Guid>, IMultiTenant, IHasConcurrencyStamp, IHasEntityVersion
{
    public Guid? TenantId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool SupportTwoFactor { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool IsActive { get; set; }

    public bool LockoutEnabled { get; set; }

    public bool IsLockedOut { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public string ConcurrencyStamp { get; set; }

    public List<string> RoleNames { get; set; }

    public int AccessFailedCount { get; set; }

    public int EntityVersion { get; set; }
}
