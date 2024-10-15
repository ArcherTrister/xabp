// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.OpenIddict.Scopes;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict.Web.Pages.OpenIddict.Scopes;

public class EditModalModel : OpenIddictProPageModel
{
  [BindProperty]
  public ScopeEditModelView Scope { get; set; }

  protected IScopeAppService ScopeAppService { get; }

  public EditModalModel(IScopeAppService scopeAppService)
  {
    ScopeAppService = scopeAppService;
  }

  public virtual async Task<IActionResult> OnGetAsync(Guid id)
  {
    var scopeDto = await ScopeAppService.GetAsync(id);
    Scope = ObjectMapper.Map<ScopeDto, ScopeEditModelView>(scopeDto);
    return Page();
  }

  public virtual async Task<IActionResult> OnPostAsync()
  {
    ValidateModel();
    var updateScopeInput = ObjectMapper.Map<ScopeEditModelView, UpdateScopeInput>(Scope);
    await ScopeAppService.UpdateAsync(Scope.Id, updateScopeInput);
    return NoContent();
  }
}
