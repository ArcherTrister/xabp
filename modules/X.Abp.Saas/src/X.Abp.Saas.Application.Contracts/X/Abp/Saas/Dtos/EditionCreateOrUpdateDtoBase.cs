// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Saas.Dtos;

public abstract class EditionCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [Display(Name = "EditionName")]
    public string DisplayName { get; set; }

    public Guid? PlanId { get; set; }

    protected EditionCreateOrUpdateDtoBase()
        : base(false)
    {
    }
}
