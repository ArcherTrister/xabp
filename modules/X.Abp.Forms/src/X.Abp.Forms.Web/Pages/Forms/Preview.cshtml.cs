// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Domain.Entities;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms;

public class PreviewModel : FormsPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    private readonly IFormAppService _formAppService;

    public PreviewModel(IFormAppService formAppService)
    {
        _formAppService = formAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var form = await _formAppService.GetAsync(Id);
        return form == null ? throw new EntityNotFoundException() : (IActionResult)Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
