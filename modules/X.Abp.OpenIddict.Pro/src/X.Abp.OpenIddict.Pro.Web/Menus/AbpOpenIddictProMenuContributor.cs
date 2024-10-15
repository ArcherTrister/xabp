// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.OpenIddict.Localization;
using Volo.Abp.UI.Navigation;

using X.Abp.OpenIddict.Permissions;

namespace X.Abp.OpenIddict.Web.Menus;

public class AbpOpenIddictProMenuContributor : IMenuContributor
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
    var localizer = context.GetLocalizer<AbpOpenIddictResource>();

    var openIddictMenuItem = new ApplicationMenuItem(AbpOpenIddictProMenus.GroupName, localizer["Menu:OpenIddict"], null, "fa fa-id-badge");

    openIddictMenuItem.AddItem(new ApplicationMenuItem(AbpOpenIddictProMenus.Applications, localizer["Applications"], "~/openIddict/Applications")
        .RequirePermissions(AbpOpenIddictProPermissions.Application.Default));

    openIddictMenuItem.AddItem(new ApplicationMenuItem(AbpOpenIddictProMenus.Scopes, localizer["Scopes"], "~/openIddict/Scopes", null)
        .RequirePermissions(AbpOpenIddictProPermissions.Scope.Default));

    context.Menu.GetAdministration().AddItem(openIddictMenuItem);

    return Task.CompletedTask;
  }
}
