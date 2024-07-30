// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using Volo.Abp.UI.Navigation;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.MainMenu
{
    public class MenuViewModel
    {
        public ApplicationMenu Menu { get; set; }

        public IList<MenuItemViewModel> Items { get; set; }
    }
}
