// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.UI.Navigation;

using X.Abp.VersionManagement.Localization;

namespace X.Abp.VersionManagement.Web.Menus;

public class AbpVersionManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        // Add main menu items.
        var localizer = context.GetLocalizer<VersionManagementResource>();
        context.Menu.AddItem(new ApplicationMenuItem(AbpVersionManagementMenus.Prefix, localizer["Menu:VersionManagement"], "~/VersionManagement", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
