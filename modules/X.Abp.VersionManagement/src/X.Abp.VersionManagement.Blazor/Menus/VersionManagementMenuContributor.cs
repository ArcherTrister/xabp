using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.VersionManagement.Blazor.Menus;

public class VersionManagementMenuContributor : IMenuContributor
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
        context.Menu.AddItem(new ApplicationMenuItem(VersionManagementMenus.Prefix, displayName: "VersionManagement", "/VersionManagement", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
