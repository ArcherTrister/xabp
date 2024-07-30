// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Content.Alerts
{
    public class ContentAlertsViewComponent : LeptonViewComponentBase
    {
        private readonly IAlertManager _alertManager;

        public ContentAlertsViewComponent(IAlertManager alertManager)
        {
            _alertManager = alertManager;
        }

        public IViewComponentResult Invoke(string name)
        {
            return View("~/Themes/Lepton/Components/Content/Alerts/Default.cshtml", _alertManager.Alerts);
        }
    }
}
