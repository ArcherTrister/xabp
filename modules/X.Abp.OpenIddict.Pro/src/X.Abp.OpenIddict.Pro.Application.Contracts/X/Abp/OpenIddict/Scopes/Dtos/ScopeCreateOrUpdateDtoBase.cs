// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;

namespace X.Abp.OpenIddict.Scopes.Dtos;

public class ScopeCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [RegularExpression("\\w+", ErrorMessage = "TheScopeNameCannotContainSpaces")]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public HashSet<string> Resources { get; set; }

    public ScopeCreateOrUpdateDtoBase()
      : base(false)
    {
    }
}
