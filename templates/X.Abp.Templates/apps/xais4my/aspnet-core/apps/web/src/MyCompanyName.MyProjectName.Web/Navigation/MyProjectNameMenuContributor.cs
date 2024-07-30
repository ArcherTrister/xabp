using System;
using System.Threading.Tasks;

using Localization.Resources.AbpUi;

using Microsoft.Extensions.Configuration;

using MyCompanyName.MyProjectName.AdministrationService.Permissions;
using MyCompanyName.MyProjectName.Localization;
using MyCompanyName.MyProjectName.ProductService.Web.Menus;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

using X.Abp.Account.Localization;
using X.Abp.AuditLogging.Web.Menus;
using X.Abp.Identity.Web.Menus;
using X.Abp.IdentityServer.Web.Menus;
using X.Abp.LanguageManagement.Web.Menus;
using X.Abp.Saas.Web.Menus;
using X.Abp.TextTemplateManagement.Web.Menus;

namespace MyCompanyName.MyProjectName.Web.Navigation;

public class MyProjectNameMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public MyProjectNameMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<MyProjectNameResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MyProjectNameMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 0
            )
        );

        //Host Dashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MyProjectNameMenus.HostDashboard,
                l["Menu:Dashboard"],
                "~/HostDashboard",
                icon: "fa fa-line-chart",
                order: 1
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Host)
        );

        //Tenant Dashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MyProjectNameMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fa fa-line-chart",
                order: 1
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Tenant)
        );

        context.Menu.SetSubItemOrder(ProductServiceMenus.ProductManagement, 2);

        context.Menu.SetSubItemOrder(AbpSaasMenuNames.GroupName, 3);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 4;

        //Administration->Identity
        administration.SetSubItemOrder(AbpIdentityProMenuNames.GroupName, 1);

        //Administration->Identity Server
        administration.SetSubItemOrder(AbpIdentityServerProMenuNames.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(AbpLanguageManagementMenusNames.GroupName, 3);

        //Administration->Text Templates
        administration.SetSubItemOrder(AbpTextTemplateManagementMenuNames.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMainMenuNames.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 6);
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var identityServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();
        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{identityServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}
