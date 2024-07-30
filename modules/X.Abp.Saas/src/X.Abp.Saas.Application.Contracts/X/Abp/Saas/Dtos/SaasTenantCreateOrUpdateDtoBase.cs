// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Saas.Dtos;

public abstract class SaasTenantCreateOrUpdateDtoBase : ExtensibleObject
{
    [Display(Name = "TenantName")]
    [StringLength(64)]
    [Required]
    public string Name { get; set; }

    public Guid? EditionId { get; set; }

    public TenantActivationState ActivationState { get; set; }

    public DateTime? ActivationEndDate { get; set; }

    public DateTime? EditionEndDateUtc { get; set; }

    protected SaasTenantCreateOrUpdateDtoBase()
        : base(false)
    {
    }
}
