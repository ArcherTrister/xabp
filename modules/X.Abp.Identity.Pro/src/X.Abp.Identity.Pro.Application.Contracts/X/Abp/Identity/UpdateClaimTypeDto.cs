// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace X.Abp.Identity;

public class UpdateClaimTypeDto : ExtensibleObject, IHasConcurrencyStamp
{
    [Required]
    public string Name { get; set; }

    public bool Required { get; set; }

    public string Regex { get; set; }

    public string RegexDescription { get; set; }

    public string Description { get; set; }

    public IdentityClaimValueType ValueType { get; set; }

    public string ConcurrencyStamp { get; set; }

    public UpdateClaimTypeDto()
        : base(false)
    {
    }
}
