// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement;

public class IndexModel : LanguageManagementPageModel
{
    public virtual Task<IActionResult> OnGetAsync() => Task.FromResult<IActionResult>(Page());

    public virtual Task<IActionResult> OnPostAsync() => Task.FromResult<IActionResult>(Page());
}
