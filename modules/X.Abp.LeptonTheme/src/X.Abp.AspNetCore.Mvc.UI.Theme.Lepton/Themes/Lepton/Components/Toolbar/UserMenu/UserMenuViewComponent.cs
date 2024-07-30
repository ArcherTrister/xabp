// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Toolbar.UserMenu
{
    public class UserMenuViewComponent : LeptonViewComponentBase
    {
        protected IMenuManager MenuManager { get; }

        public UserMenuViewComponent(IMenuManager menuManager)
        {
            MenuManager = menuManager;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var menu = await MenuManager.GetAsync(StandardMenus.User);
            return View("~/Themes/Lepton/Components/Toolbar/UserMenu/Default.cshtml", menu);
        }
    }
}
