// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.OpenIddict.Scopes;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict.Pro.Web.Pages.OpenIddict.Scopes;

public class CreateModalModel : OpenIddictProPageModel
{
    [BindProperty]
    public ScopeCreateModalView Scope { get; set; }

    protected virtual IScopeAppService ScopeAppService { get; }

    public CreateModalModel(IScopeAppService scopeAppService)
    {
        ScopeAppService = scopeAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        Scope = new ScopeCreateModalView();
        return Task.FromResult((IActionResult)Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        var createScopeInput = ObjectMapper.Map<ScopeCreateModalView, CreateScopeInput>(Scope);
        await ScopeAppService.CreateAsync(createScopeInput);
        return NoContent();
    }
}
