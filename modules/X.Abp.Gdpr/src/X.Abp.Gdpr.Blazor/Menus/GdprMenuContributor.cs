using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.Gdpr.Blazor.Menus;

public class GdprMenuContributor : IMenuContributor
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
        context.Menu.AddItem(new ApplicationMenuItem(GdprMenus.Prefix, displayName: "Gdpr", "/Gdpr", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
