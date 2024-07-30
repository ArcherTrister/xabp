// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class ImpersonateTenantModalModel : SaasPageModel
{
    public string ReturnUrl { get; } = "/Saas/Tenants";

    public Guid TenantId { get; set; }

    public string DefaultAdminUserName { get; set; } = "admin";

    public void OnGet(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
