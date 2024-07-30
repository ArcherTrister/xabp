// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Identity;

public class GetIdentityUsersInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public Guid? RoleId { get; set; }

    public Guid? OrganizationUnitId { get; set; }

    public string UserName { get; set; }

    public string PhoneNumber { get; set; }

    public string EmailAddress { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public bool? IsLockedOut { get; set; }

    public bool? NotActive { get; set; }

    public bool? EmailConfirmed { get; set; }

    public bool? IsExternal { get; set; }

    public DateTime? MaxCreationTime { get; set; }

    public DateTime? MinCreationTime { get; set; }

    public DateTime? MaxModifitionTime { get; set; }

    public DateTime? MinModifitionTime { get; set; }
}
