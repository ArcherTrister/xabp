// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

namespace X.Abp.Account.Dtos;

public class ProfileDto : ExtensibleObject, IHasConcurrencyStamp
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool IsExternal { get; set; }

    public bool HasPassword { get; set; }

    public string ConcurrencyStamp { get; set; }
}
