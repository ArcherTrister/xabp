// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.Identity;

public class IdentityRoleCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
    public string Name { get; set; }

    /// <summary>
    /// Display name of this role.
    /// </summary>
    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
    public string DisplayName { get; set; }

    public bool IsDefault { get; set; }

    public bool IsPublic { get; set; }

    protected IdentityRoleCreateOrUpdateDtoBase()
        : base(false)
    {
    }
}
