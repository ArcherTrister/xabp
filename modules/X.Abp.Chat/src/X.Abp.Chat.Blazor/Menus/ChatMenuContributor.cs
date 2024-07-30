using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace X.Abp.Chat.Blazor.Menus;

public class ChatMenuContributor : IMenuContributor
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
        context.Menu.AddItem(new ApplicationMenuItem(ChatMenus.Prefix, displayName: "Chat", "/Chat", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
