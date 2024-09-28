// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.UI.Navigation;

using X.Abp.LeptonTheme.Management;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Menus
{
    public class MenuViewModel
    {
        public ApplicationMenu Menu { get; set; }

        public IList<MenuItemViewModel> Items { get; set; }

        public MenuPlacement Placement { get; set; }

        public MenuStatus NavBarStatus { get; set; }

        public EventHandler StateChanged { get; set; }

        public void SetParents()
        {
            foreach (var item in Items)
            {
                item.SetParents(null);
            }
        }

        public void ToggleOpen(MenuItemViewModel menuItem)
        {
            if (menuItem.IsOpen)
            {
                menuItem.Close();
            }
            else
            {
                CloseAll();
                menuItem.Open();
            }

            StateChanged.InvokeSafely(this);
        }

        public void Activate(MenuItemViewModel menuItem)
        {
            if (menuItem.IsActive)
            {
                return;
            }

            DeactivateAll();
            menuItem.Open();
            menuItem.Activate();
            StateChanged.InvokeSafely(this);
        }

        public void ToggleNavbarStatus()
        {
            if (NavBarStatus == MenuStatus.AlwaysOpened)
            {
                NavBarStatus = MenuStatus.OpenOnHover;
            }
            else
            {
                NavBarStatus = MenuStatus.AlwaysOpened;
            }
        }

        private void CloseAll()
        {
            foreach (var item in Items)
            {
                item.Close();
            }
        }

        private void DeactivateAll()
        {
            foreach (var item in Items)
            {
                item.Deactivate();
            }
        }
    }
}
