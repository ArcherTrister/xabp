using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.LeptonTheme.Management.Web
{
  public class AbpLeptonThemeManagementMenuContributor : IMenuContributor
  {
    public virtual async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
      if (context.Menu.Name == StandardMenus.Main)
      {
        await ConfigureMainMenuAsync(context);
      }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
      //Add main menu items.

      return Task.CompletedTask;
    }
  }
}
