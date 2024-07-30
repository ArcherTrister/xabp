// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Identity;

public class IdentityOrganizationUnitSettingsDto
{
    /// <summary>
    /// 组织单位最大允许的成员资格计数
    /// </summary>
    [Display(Name = "Abp.Identity.OrganizationUnit.MaxUserMembershipCount")]
    public int MaxUserMembershipCount { get; set; }
}
