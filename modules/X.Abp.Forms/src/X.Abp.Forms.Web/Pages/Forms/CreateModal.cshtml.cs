// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms;

public class CreateModalModel : FormsPageModel
{
    [BindProperty]
    public CreateFormDto Form { get; set; }

    protected IFormAppService FormAppService { get; }

    public CreateModalModel(IFormAppService formAppService)
    {
        FormAppService = formAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var createdForm = await FormAppService.CreateAsync(Form);

        var createdUrl = $"Forms/{createdForm.Id}/Questions";
        return Content(createdUrl);
    }
}
