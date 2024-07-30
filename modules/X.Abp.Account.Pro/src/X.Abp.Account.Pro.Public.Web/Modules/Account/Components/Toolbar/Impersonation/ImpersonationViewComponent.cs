// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Account.Public.Web.Modules.Account.Components.Toolbar.Impersonation;

public class ImpersonationViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Modules/Account/Components/Toolbar/Impersonation/Default.cshtml");
    }
}
