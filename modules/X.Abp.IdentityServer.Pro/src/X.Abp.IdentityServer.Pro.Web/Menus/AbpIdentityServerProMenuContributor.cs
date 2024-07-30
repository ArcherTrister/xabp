// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.IdentityServer.Localization;
using Volo.Abp.UI.Navigation;

using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer.Web.Menus
{
    public class AbpIdentityServerProMenuContributor : IMenuContributor
    {
        public virtual async
        Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == "Main")
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        protected virtual Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            IStringLocalizer localizer = context.GetLocalizer<AbpIdentityServerResource>();

            ApplicationMenuItem menuItem = new ApplicationMenuItem(AbpIdentityServerProMenuNames.GroupName, localizer["Menu:IdentityServer"], icon: "fa fa-server");
            context.Menu.GetAdministration().AddItem(menuItem);
            menuItem.AddItem(new ApplicationMenuItem(AbpIdentityServerProMenuNames.Clients, localizer["Menu:Clients"], "~/IdentityServer/Clients").RequirePermissions(AbpIdentityServerProPermissions.Client.Default));
            menuItem.AddItem(new ApplicationMenuItem(AbpIdentityServerProMenuNames.IdentityResources, localizer["Menu:IdentityResources"], "~/IdentityServer/IdentityResources").RequirePermissions(AbpIdentityServerProPermissions.IdentityResource.Default));
            menuItem.AddItem(new ApplicationMenuItem(AbpIdentityServerProMenuNames.ApiResources, localizer["Menu:ApiResources"], "~/IdentityServer/ApiResources").RequirePermissions(AbpIdentityServerProPermissions.ApiResource.Default));
            menuItem.AddItem(new ApplicationMenuItem(AbpIdentityServerProMenuNames.ApiScopes, localizer["Menu:ApiScopes"], "~/IdentityServer/ApiScopes").RequirePermissions(AbpIdentityServerProPermissions.ApiScope.Default));
            return Task.CompletedTask;
        }
    }
}
