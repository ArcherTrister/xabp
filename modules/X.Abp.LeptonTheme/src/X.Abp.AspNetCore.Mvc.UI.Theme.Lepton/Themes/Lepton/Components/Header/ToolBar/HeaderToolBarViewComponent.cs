// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Header.ToolBar
{
    public class HeaderToolBarViewComponent : LeptonViewComponentBase
    {
        protected IToolbarManager ToolbarManager { get; }

        public HeaderToolBarViewComponent(IToolbarManager toolbarManager)
        {
            ToolbarManager = toolbarManager;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var toolbar = await ToolbarManager.GetAsync(StandardToolbars.Main);
            return View("~/Themes/Lepton/Components/Header/ToolBar/Default.cshtml", toolbar);
        }
    }
}
