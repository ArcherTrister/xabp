using System;
using System.Threading.Tasks;
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

            var menuItemPath = MenuItem.MenuItem.Url.Replace("~/", string.Empty).Trim('/');
            var currentPagePath = new Uri(NavigationManager.Uri.TrimEnd('/')).AbsolutePath.Trim('/');

            if (menuItemPath.TrimEnd('/').Equals(currentPagePath, StringComparison.InvariantCultureIgnoreCase))
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
