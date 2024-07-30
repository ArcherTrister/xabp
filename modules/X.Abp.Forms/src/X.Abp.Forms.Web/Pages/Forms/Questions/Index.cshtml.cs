// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms.Questions;

public class IndexModel : FormsPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    protected IFormAppService FormAppService { get; }

    public long ResponseCount { get; set; }

    public IndexModel(IFormAppService formAppService)
    {
        FormAppService = formAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var form = await FormAppService.GetAsync(Id);
        if (form.Id == Guid.Empty)
        {
            return NotFound();
        }

        ResponseCount = await FormAppService.GetResponsesCountAsync(Id);

        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
