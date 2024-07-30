// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Header
{
    public class HeaderViewComponent : LeptonViewComponentBase
    {
        public virtual IViewComponentResult Invoke()
        {
            return View("~/Themes/Lepton/Components/Header/Default.cshtml");
        }
    }
}
