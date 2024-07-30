using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Web.Menus;

public class AdminMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(AdminMenus.Prefix, displayName: "Admin", "~/Admin", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
