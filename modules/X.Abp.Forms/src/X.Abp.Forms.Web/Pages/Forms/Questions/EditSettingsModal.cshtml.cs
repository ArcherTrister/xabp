// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms.Questions;

public class EditSettingsModalModel : FormsPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateFormSettingInputDto FormSettings { get; set; }

    protected IFormAppService FormAppService { get; }

    public EditSettingsModalModel(IFormAppService formAppService)
    {
        FormAppService = formAppService;
    }

    public virtual async Task OnGetAsync()
    {
        var settings = await FormAppService.GetSettingsAsync(Id);

        FormSettings = ObjectMapper.Map<FormSettingsDto, UpdateFormSettingInputDto>(settings);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        await FormAppService.SetSettingsAsync(Id, FormSettings);

        return NoContent();
    }
}
