// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web.Pages.Shared.Components.SaasLatestTenantsWidget;

[Widget(RefreshUrl = "SaasWidgets/LatestTenants", RequiredPolicies = new string[] { "Saas.Tenants" })]
public class SaasLatestTenantsWidgetViewComponent : SaasViewComponent
{
    protected ITenantAppService TenantAppService { get; }

    public SaasLatestTenantsWidgetViewComponent(ITenantAppService tenantAppService)
    {
        TenantAppService = tenantAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var input = new GetTenantsInput
        {
            GetEditionNames = true,
            MaxResultCount = 6,
            SkipCount = 0,
            Sorting = "CreationTime desc"
        };
        var model = await TenantAppService.GetListAsync(input);
        return View("/Pages/Shared/Components/SaasLatestTenantsWidget/Default.cshtml", model);
    }
}
