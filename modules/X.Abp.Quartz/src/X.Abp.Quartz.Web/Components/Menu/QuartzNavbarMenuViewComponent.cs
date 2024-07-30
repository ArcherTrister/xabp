// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation;

using X.Abp.Quartz.Web.Menus;

namespace X.Abp.Quartz.Web.Components.Menu;

public class QuartzNavbarMenuViewComponent : AbpViewComponent
{
    protected IMenuManager MenuManager { get; }

    public QuartzNavbarMenuViewComponent(IMenuManager menuManager)
    {
        MenuManager = menuManager;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var menu = await MenuManager.GetAsync(AbpQuartzMenus.Prefix);
        return View("~/Components/Menu/Default.cshtml", menu);
    }
}
