using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.IdentityServer.Blazor.Menus;

public class ProMenuContributor : IMenuContributor
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
    context.Menu.AddItem(new ApplicationMenuItem(ProMenus.Prefix, displayName: "Pro", "/Pro", icon: "fa fa-globe"));

    return Task.CompletedTask;
  }
}
