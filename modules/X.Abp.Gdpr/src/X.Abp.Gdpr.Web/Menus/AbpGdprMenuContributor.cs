// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

using X.Abp.Gdpr.Localization;

namespace X.Abp.Gdpr.Web.Menus;

public class AbpGdprMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var localizer = context.GetLocalizer<AbpGdprResource>();
        context.Menu.Items.AddIfNotContains(
            new ApplicationMenuItem(AbpGdprMenus.GdprMenuNames.PersonalData, localizer["Menu:PersonalData"], "/Gdpr/PersonalData", "fa fa-lock").RequireAuthenticated());

        return Task.CompletedTask;
    }
}
