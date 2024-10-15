// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using X.Abp.OpenIddict.Applications;
using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Scopes;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict.Web.Pages.OpenIddict.Applications;

public class CreateModalModel : OpenIddictProPageModel
{
    protected IScopeAppService ScopeAppService { get; }

    protected IApplicationAppService ApplicationAppService { get; }

    public List<ScopeDto> Scopes { get; set; }

    public List<SelectListItem> Types { get; set; }

    public List<SelectListItem> ConsentTypes { get; set; }

    [BindProperty]
    public ApplicationCreateModalView Application { get; set; }

    public CreateModalModel(
      IScopeAppService scopeAppService,
      IApplicationAppService applicationAppService)
    {
        ScopeAppService = scopeAppService;
        ApplicationAppService = applicationAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        Application = new ApplicationCreateModalView();
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
        var applicationInput = ObjectMapper.Map<ApplicationCreateModalView, CreateApplicationInput>(Application);
        await ApplicationAppService.CreateAsync(applicationInput);
        return NoContent();
    }
}
