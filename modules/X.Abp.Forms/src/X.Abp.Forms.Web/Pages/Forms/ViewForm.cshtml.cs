// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Forms.Responses;

namespace X.Abp.Forms.Web.Pages.Forms;

public class ViewFormModel : FormsPageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    protected IResponseAppService ResponseAppService { get; }

    public string Title { get; set; }

    public ViewFormModel(IResponseAppService responseAppService)
    {
        ResponseAppService = responseAppService;
    }

    public virtual async Task<IActionResult> OnGet()
    {
        var form = await ResponseAppService.GetFormDetailsAsync(Id);

        Title = form.Title;

        return !form.RequiresLogin
            ? await Task.FromResult<IActionResult>(Page())
            : !CurrentUser.IsAuthenticated
            ? RedirectToPage("/Account/Login", new
            {
                ReturnUrl = $"/Forms/{Id}/ViewForm"
            })
            : await Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
