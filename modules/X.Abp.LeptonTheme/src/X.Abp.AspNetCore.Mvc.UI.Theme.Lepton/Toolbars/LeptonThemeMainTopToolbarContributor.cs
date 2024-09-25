// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Localization;
using Volo.Abp.Users;

using X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Toolbar.LanguageSwitch;
using X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Themes.Lepton.Components.Toolbar.UserMenu;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Toolbars
{
  public class LeptonThemeMainTopToolbarContributor : IToolbarContributor
  {
    public virtual async Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
    {
      if (context.Toolbar.Name != StandardToolbars.Main)
      {
        return;
      }

      if (!(context.Theme is LeptonTheme))
      {
        return;
      }

      var languageProvider = context.ServiceProvider.GetRequiredService<ILanguageProvider>();

      // TODO: This duplicates GetLanguagesAsync() usage. Can we eleminate this?
      var languages = await languageProvider.GetLanguagesAsync();
      if (languages.Count > 1)
      {
        context.Toolbar.Items.Add(new ToolbarItem(typeof(LanguageSwitchViewComponent)));
      }

      if (context.ServiceProvider.GetRequiredService<ICurrentUser>().IsAuthenticated)
      {
        context.Toolbar.Items.Add(new ToolbarItem(typeof(UserMenuViewComponent)));
      }
    }
  }
}
