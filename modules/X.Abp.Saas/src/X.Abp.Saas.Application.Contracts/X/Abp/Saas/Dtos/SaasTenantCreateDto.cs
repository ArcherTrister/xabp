// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Auditing;

namespace X.Abp.Saas.Dtos;

public class SaasTenantCreateDto : SaasTenantCreateOrUpdateDtoBase
{
    [StringLength(256)]
    [Required]
    [EmailAddress]
    public string AdminEmailAddress { get; set; }

    [StringLength(128)]
    [Required]
    [DisableAuditing]
    public string AdminPassword { get; set; }

    public SaasTenantConnectionStringsDto ConnectionStrings { get; set; }
}
