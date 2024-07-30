// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

using X.Abp.Saas.Localization;

namespace X.Abp.Saas.Web.Menus;

public class AbpSaasMenuContributor : IMenuContributor
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
        var localizer = context.GetLocalizer<SaasResource>();
        var menuItem = new ApplicationMenuItem(AbpSaasMenuNames.GroupName, localizer["Menu:Saas"], null, "fa fa-globe");
        context.Menu.AddItem(menuItem);
        menuItem.AddItem(new ApplicationMenuItem(AbpSaasMenuNames.Tenants, localizer["Tenants"], "~/Saas/Tenants", "bi bi-people-fill").RequirePermissions("Saas.Tenants"));
        menuItem.AddItem(new ApplicationMenuItem(AbpSaasMenuNames.Editions, localizer["Editions"], "~/Saas/Editions", "bi bi-ui-checks-grid").RequirePermissions("Saas.Editions"));
        return Task.CompletedTask;
    }
}
