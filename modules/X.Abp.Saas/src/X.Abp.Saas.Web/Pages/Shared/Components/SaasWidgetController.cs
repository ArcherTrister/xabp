// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Saas.Web.Pages.Shared.Components.SaasLatestTenantsWidget;

namespace X.Abp.Saas.Web.Pages.Shared.Components;

[ApiExplorerSettings(IgnoreApi = true)]
[Area("saas")]
[Route("SaasWidgets")]
[RemoteService(false)]
public class SaasWidgetController : AbpController
{
    public SaasWidgetController()
    {
        ObjectMapperContext = typeof(AbpSaasWebModule);
    }

    [HttpGet]
    [Route("LatestTenants")]
    public virtual IActionResult LatestTenants()
    {
        return ViewComponent(typeof(SaasLatestTenantsWidgetViewComponent));
    }
}
