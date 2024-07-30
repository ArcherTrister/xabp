// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Content.BreadCrumb
{
    public class ContentBreadCrumbViewComponent : LeptonViewComponentBase
    {
        protected IPageLayout PageLayout { get; }

        public ContentBreadCrumbViewComponent(IPageLayout pageLayout)
        {
            PageLayout = pageLayout;
        }

        public virtual IViewComponentResult Invoke()
        {
            return View("~/Themes/Lepton/Components/Content/BreadCrumb/Default.cshtml", PageLayout.Content);
        }
    }
}
