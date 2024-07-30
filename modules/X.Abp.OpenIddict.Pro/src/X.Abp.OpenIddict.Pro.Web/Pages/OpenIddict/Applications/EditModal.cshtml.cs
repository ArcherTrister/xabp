// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using X.Abp.OpenIddict.Applications;
using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Scopes;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict.Pro.Web.Pages.OpenIddict.Applications;

public class EditModalModel : OpenIddictProPageModel
{
    protected IScopeAppService ScopeAppService { get; }

    protected IApplicationAppService ApplicationAppService { get; }

    [BindProperty]
    public ApplicationEditModalView Application { get; set; }

    public List<ScopeDto> Scopes { get; set; }

    public List<SelectListItem> Types { get; set; }

    public List<SelectListItem> ConsentTypes { get; set; }

    public EditModalModel(IScopeAppService scopeAppService, IApplicationAppService applicationAppService)
    {
        ScopeAppService = scopeAppService;
        ApplicationAppService = applicationAppService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var applicationDto = await ApplicationAppService.GetAsync(id);
        Application = ObjectMapper.Map<ApplicationDto, ApplicationEditModalView>(applicationDto);
        Scopes = await ScopeAppService.GetAllScopesAsync();
        Types = new List<SelectListItem>()
        {
          new SelectListItem("Confidential client", "confidential"),
          new SelectListItem("Public client", "public")
        };
        ConsentTypes = new List<SelectListItem>()
        {
          new SelectListItem("Explicit consent", "explicit"),
          new SelectListItem("External consent", "external"),
          new SelectListItem("Implicit consent", "implicit"),
          new SelectListItem("Systematic consent", "systematic")
        };
        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var applicationInput = ObjectMapper.Map<ApplicationEditModalView, UpdateApplicationInput>(Application);
        await ApplicationAppService.UpdateAsync(Application.Id, applicationInput);
        return NoContent();
    }
}
