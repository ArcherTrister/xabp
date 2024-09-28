// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.UI.Navigation;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Menus
{
  public class MainMenuProvider : IScopedDependency
  {
    private readonly IMenuManager _menuManager;
    private readonly IObjectMapper<AbpAspNetCoreComponentsWebLeptonThemeModule> _objectMapper;
    private readonly ILeptonSettingsProvider _leptonSettings;

    private MenuViewModel Menu { get; set; }

    public MainMenuProvider(
        IMenuManager menuManager,
        IObjectMapper<AbpAspNetCoreComponentsWebLeptonThemeModule> objectMapper,
        ILeptonSettingsProvider leptonSettings)
    {
      _menuManager = menuManager;
      _objectMapper = objectMapper;
      _leptonSettings = leptonSettings;
    }

    public virtual async Task<MenuViewModel> GetMenuAsync()
    {
      if (Menu == null)
      {
        var menu = await _menuManager.GetMainMenuAsync();
        Menu = _objectMapper.Map<ApplicationMenu, MenuViewModel>(menu);
        Menu.SetParents();
        Menu.Placement = await _leptonSettings.GetMenuPlacementAsync();
        Menu.NavBarStatus = await _leptonSettings.GetMenuStatusAsync();
      }

      return Menu;
    }
  }
}
