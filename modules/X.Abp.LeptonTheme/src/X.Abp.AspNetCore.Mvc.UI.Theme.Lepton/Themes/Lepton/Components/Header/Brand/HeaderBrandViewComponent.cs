// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Header.Brand
{
    public class HeaderBrandViewComponent : LeptonViewComponentBase
    {
        public virtual IViewComponentResult Invoke()
        {
            return View("~/Themes/Lepton/Components/Header/Brand/Default.cshtml");
        }
    }
}
