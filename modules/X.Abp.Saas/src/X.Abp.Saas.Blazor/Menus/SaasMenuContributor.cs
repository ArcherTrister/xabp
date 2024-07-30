using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.Saas.Blazor.Menus;

public class SaasMenuContributor : IMenuContributor
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
        context.Menu.AddItem(new ApplicationMenuItem(SaasMenus.Prefix, displayName: "Saas", "/Saas", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
