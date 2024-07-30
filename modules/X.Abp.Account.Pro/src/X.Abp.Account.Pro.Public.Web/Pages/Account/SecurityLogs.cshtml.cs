// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class SecurityLogsModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [Display(Name = "MySecurityLogs:StartTime")]
    public DateTime? StartTime { get; set; }

    [Display(Name = "MySecurityLogs:EndTime")]
    public DateTime? EndTime { get; set; }

    [Display(Name = "MySecurityLogs:Action")]
    public string Action { get; set; }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
