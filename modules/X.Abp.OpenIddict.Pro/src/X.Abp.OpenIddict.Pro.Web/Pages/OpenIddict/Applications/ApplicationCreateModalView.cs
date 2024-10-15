// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.ObjectExtending;

namespace X.Abp.OpenIddict.Web.Pages.OpenIddict.Applications;

public class ApplicationCreateModalView : ExtensibleObject
{
    [Required]
    public string ClientId { get; set; }

    [Required]
    public string DisplayName { get; set; }

    public string Type { get; set; }

    public string ClientSecret { get; set; }

    public string ConsentType { get; set; }

    public string ClientUri { get; set; }

    public string LogoUri { get; set; }

    [TextArea]
    public string PostLogoutRedirectUris { get; set; }

    [TextArea]
    public string RedirectUris { get; set; }

    public bool AllowPasswordFlow { get; set; }

    public bool AllowClientCredentialsFlow { get; set; }

    public bool AllowAuthorizationCodeFlow { get; set; }

    public bool AllowRefreshTokenFlow { get; set; }

    public bool AllowHybridFlow { get; set; }

    public bool AllowImplicitFlow { get; set; }

    public bool AllowLogoutEndpoint { get; set; }

    public bool AllowDeviceEndpoint { get; set; }

    public HashSet<string> Scopes { get; set; }
}
