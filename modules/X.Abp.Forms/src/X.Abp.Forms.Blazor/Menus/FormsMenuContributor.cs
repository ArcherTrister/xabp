using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.Forms.Blazor.Menus;

public class FormsMenuContributor : IMenuContributor
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
    context.Menu.AddItem(new ApplicationMenuItem(FormsMenus.Prefix, displayName: "Forms", "/Forms", icon: "fa fa-globe"));

    return Task.CompletedTask;
  }
}
