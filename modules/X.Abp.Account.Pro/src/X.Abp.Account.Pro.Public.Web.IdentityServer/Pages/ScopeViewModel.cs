// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.Account.Web.Pages;

public class ScopeViewModel
{
    [Required]
    [HiddenInput]
    public string Name { get; set; }

    public bool Checked { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool Emphasize { get; set; }

    public bool Required { get; set; }
}
