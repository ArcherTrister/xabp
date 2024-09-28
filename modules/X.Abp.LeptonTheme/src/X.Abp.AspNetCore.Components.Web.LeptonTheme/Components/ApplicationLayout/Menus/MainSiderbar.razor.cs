// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Menus
{
    public partial class MainSiderbar : IDisposable
    {
        [Inject]
        protected MainMenuProvider MainMenuProvider { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected MenuViewModel Menu { get; set; }

        [Parameter]
        public EventCallback OnClickCallback { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetMenuAsync();
            AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProviderOnAuthenticationStateChanged;
        }

        private async Task GetMenuAsync()
        {
            Menu = await MainMenuProvider.GetMenuAsync();
            Menu.StateChanged += Menu_StateChanged;
        }

        private async void AuthenticationStateProviderOnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await GetMenuAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void Menu_StateChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Menu.StateChanged -= Menu_StateChanged;
            AuthenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateProviderOnAuthenticationStateChanged;
        }
    }
}
