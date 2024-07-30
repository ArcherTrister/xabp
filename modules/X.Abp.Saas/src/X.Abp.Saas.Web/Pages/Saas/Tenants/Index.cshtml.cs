// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using X.Abp.Saas;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class IndexModel : SaasPageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? EditionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ExpirationDateMin { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ExpirationDateMax { get; set; }

    [BindProperty(SupportsGet = true)]
    public TenantActivationState? ActivationState { get; set; }

    public List<SelectListItem> EditionsComboboxItems { get; set; } = new List<SelectListItem>()
    {
        new SelectListItem(string.Empty, string.Empty, true)
    };

    protected ITenantAppService TenantAppService { get; }

    public IndexModel(ITenantAppService tenantAppService)
    {
        TenantAppService = tenantAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var source = await TenantAppService.GetEditionLookupAsync();
        EditionsComboboxItems.AddRange(source.Select(e => new SelectListItem(e.DisplayName, e.Id.ToString())).ToList());
        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
