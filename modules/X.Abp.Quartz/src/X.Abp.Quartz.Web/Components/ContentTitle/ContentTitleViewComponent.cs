// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;

namespace X.Abp.Quartz.Web.Components.ContentTitle;

public class ContentTitleViewComponent : AbpViewComponent
{
    protected IPageLayout PageLayout { get; }

    public ContentTitleViewComponent(IPageLayout pageLayout)
    {
        PageLayout = pageLayout;
    }

    public virtual IViewComponentResult Invoke()
    {
        return View("~/Components/ContentTitle/Default.cshtml", PageLayout.Content);
    }
}
