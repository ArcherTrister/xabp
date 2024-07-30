// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Identity;

public class IdentityOAuthSettingsDto
{
    [Display(Name = "DisplayName:Abp.Identity.EnableOAuthLogin")]
    public bool EnableOAuthLogin { get; set; }

    [Required]
    [Display(Name = "DisplayName:Abp.Identity.ClientId")]
    public string ClientId { get; set; }

    [Display(Name = "DisplayName:Abp.Identity.ClientSecret")]
    public string ClientSecret { get; set; }

    [Required]
    [Display(Name = "DisplayName:Abp.Identity.Authority")]
    public string Authority { get; set; }

    [Display(Name = "DisplayName:Abp.Identity.Scope")]
    public string Scope { get; set; }

    [Display(Name = "DisplayName:Abp.Identity.RequireHttpsMetadata")]
    public bool RequireHttpsMetadata { get; set; }
}
