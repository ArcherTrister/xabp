// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.ObjectExtending;

namespace X.Abp.OpenIddict.Web.Pages.OpenIddict.Scopes;

public class ScopeCreateModalView : ExtensibleObject
{
    [RegularExpression("\\w+", ErrorMessage = "TheScopeNameCannotContainSpaces")]
    [Required]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    [TextArea]
    public string Resources { get; set; }
}
