// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Menus
{
    public partial class MainSiderbarMenuItem : IDisposable
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public MenuViewModel Menu { get; set; }

        [Parameter]
        public MenuItemViewModel MenuItem { get; set; }

        protected override void OnParametersSet()
        {
            ActivateCurrentPage();
        }

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        protected virtual void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            ActivateCurrentPage();
        }

        protected virtual void ActivateCurrentPage()
        {
            if (MenuItem.MenuItem.Url.IsNullOrEmpty())
            {
                return;
            }

            var menuItemPath = MenuItem.MenuItem.Url.Replace("~/", string.Empty, StringComparison.OrdinalIgnoreCase).Trim('/');
            var currentPagePath = new Uri(NavigationManager.Uri.TrimEnd('/')).AbsolutePath.Trim('/');

            if (menuItemPath.TrimEnd('/').Equals(currentPagePath, StringComparison.OrdinalIgnoreCase))
            {
                Menu.Activate(MenuItem);
            }
        }

        protected virtual void ToggleMenu()
        {
            Menu.ToggleOpen(MenuItem);
        }

        public virtual void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
