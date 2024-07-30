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

        public async Task<MenuViewModel> GetMenuAsync()
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
