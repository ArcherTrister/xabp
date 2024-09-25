// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Localization.Resources.AbpUi;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

using X.Abp.Account.AuthorityDelegation;
using X.Abp.Account.Localization;

namespace X.Abp.Account.Public.Web.Menus;

public class AbpAccountUserMenuContributor : IMenuContributor
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

        ICurrentUser currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        IOptions<AbpAccountAuthorityDelegationOptions> authorityDelegationOptions = context.ServiceProvider.GetRequiredService<IOptions<AbpAccountAuthorityDelegationOptions>>();

        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.LinkedAccounts, accountLocalizer["LinkedAccounts"], url: "javascript:void(0)", icon: "fa fa-link"));
        if (!currentUser.FindImpersonatorUserId().HasValue && authorityDelegationOptions.Value.EnableDelegatedImpersonation)
        {
            context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.AuthorityDelegation, accountLocalizer["AuthorityDelegation"], url: "javascript:void(0)", icon: "fa fa-users"));
        }

        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.Manage, accountLocalizer["MyAccount"], url: "~/Account/Manage", icon: "fa fa-cog"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.SecurityLogs, accountLocalizer["MySecurityLogs"], url: "~/Account/SecurityLogs", icon: "fa fa-shield"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.Sessions, accountLocalizer["Sessions"], url: "~/Account/Sessions", icon: "fa fa-clock"));
        context.Menu.AddItem(new ApplicationMenuItem(AbpAccountPublicMenus.Logout, uiLocalizer["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue));

        return Task.CompletedTask;
    }
}
