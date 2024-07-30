// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace X.Abp.Saas.Web.Pages.Shared.Components.SaasEditionPercentageWidget;

[Widget(RequiredPolicies = new string[] { "Saas.Tenants" }, ScriptTypes = new Type[] { typeof(SaasEditionPercentageWidgetScriptContributor) })]
public class SaasEditionPercentageWidgetViewComponent : SaasViewComponent
{
    public virtual Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult<IViewComponentResult>(View("/Pages/Shared/Components/SaasEditionPercentageWidget/Default.cshtml"));
    }
}
