// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.AuditLogging.Permissions;

namespace X.Abp.AuditLogging.Web.Pages.Shared.Components.AverageExecutionDurationPerDayWidget
{
    [Widget(RequiredPolicies = new string[] { AbpAuditLoggingPermissions.AuditLogs.Default }, ScriptTypes = new Type[] { typeof(AuditLoggingAverageExecutionDurationPerDayWidgetScriptContributor) })]
    public class AuditLoggingAverageExecutionDurationPerDayWidgetViewComponent : AuditLogsComponentBase
    {
        public virtual IViewComponentResult Invoke()
        {
            return View("/Pages/Shared/Components/AverageExecutionDurationPerDayWidget/Default.cshtml");
        }
    }
}
