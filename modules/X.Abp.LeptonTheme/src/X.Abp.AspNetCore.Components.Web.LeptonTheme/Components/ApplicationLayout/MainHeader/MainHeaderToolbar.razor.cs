// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.MainHeader
{
    public partial class MainHeaderToolbar
    {
        [Inject]
        private IToolbarManager ToolbarManager { get; set; }

        [Inject]
        private IAbpUtilsService AbpUtilsService { get; set; }

        private RenderFragment ToolbarRender { get; set; }

        private List<RenderFragment> ToolbarItemRenders { get; set; } = new List<RenderFragment>();

        private bool IsFullScreen { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var toolbar = await ToolbarManager.GetAsync(StandardToolbars.Main);

            ToolbarItemRenders.Clear();

            foreach (var item in toolbar.Items.OrderBy(i => i.Order))
            {
                ToolbarItemRenders.Add(builder =>
                {
                    builder.OpenComponent(0, item.ComponentType);
                    builder.CloseComponent();
                });
            }
        }

        private async Task ToogleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            if (IsFullScreen)
            {
                await AbpUtilsService.RequestFullscreenAsync();
            }
            else
            {
                await AbpUtilsService.ExitFullscreenAsync();
            }
        }
    }
}
