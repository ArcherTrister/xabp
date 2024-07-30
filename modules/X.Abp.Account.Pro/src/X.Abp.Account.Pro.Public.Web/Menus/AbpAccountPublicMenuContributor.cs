// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Localization.Resources.AbpUi;

using Volo.Abp.UI.Navigation;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Public.Web.Menus;

public class AbpAccountPublicMenuContributor : IMenuContributor
{
    public virtual async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var uiLocalizer = context.GetLocalizer<AbpUiResource>();
        var accountLocalizer = context.GetLocalizer<AccountResource>();

        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.LinkedAccounts, accountLocalizer["LinkedAccounts"], url: "javascript:void(0)", icon: "fa fa-link", cssClass: "d-none"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.Manage, accountLocalizer["MyAccount"], url: "~/Account/Manage", icon: "fa fa-cog"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.SecurityLogs, accountLocalizer["MySecurityLogs"], url: "~/Account/SecurityLogs", icon: "fa fa-cog"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.Logout, uiLocalizer["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000));

        return Task.CompletedTask;
    }
}
