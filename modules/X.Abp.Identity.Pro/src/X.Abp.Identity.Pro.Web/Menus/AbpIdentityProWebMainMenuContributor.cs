// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Localization;
using Volo.Abp.UI.Navigation;

using X.Abp.Identity.Permissions;

namespace X.Abp.Identity.Web.Menus;

public class AbpIdentityProWebMainMenuContributor : IMenuContributor
{
    public virtual async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var localizer = context.GetLocalizer<IdentityResource>();

        var identityMenuItem = new ApplicationMenuItem(AbpIdentityProMenuNames.GroupName, localizer["Menu:IdentityManagement"], icon: "fa fa-id-card-o");

        identityMenuItem.AddItem(new ApplicationMenuItem(AbpIdentityProMenuNames.OrganizationUnits, localizer["OrganizationUnits"], url: "~/Identity/OrganizationUnits").RequirePermissions(AbpIdentityProPermissions.OrganizationUnits.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(AbpIdentityProMenuNames.Roles, localizer["Roles"], url: "~/Identity/Roles").RequirePermissions(AbpIdentityProPermissions.Roles.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(AbpIdentityProMenuNames.Users, localizer["Users"], url: "~/Identity/Users").RequirePermissions(AbpIdentityProPermissions.Users.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(AbpIdentityProMenuNames.ClaimTypes, localizer["ClaimTypes"], url: "~/Identity/ClaimTypes").RequirePermissions(AbpIdentityProPermissions.ClaimTypes.Default));
        identityMenuItem.AddItem(new ApplicationMenuItem(AbpIdentityProMenuNames.SecurityLogs, localizer["SecurityLogs"], url: "~/Identity/SecurityLogs").RequirePermissions(AbpIdentityProPermissions.SecurityLogs.Default));

        context.Menu.GetAdministration().AddItem(identityMenuItem);

        return Task.CompletedTask;
    }
}
