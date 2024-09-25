// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.UI.Navigation;

namespace X.Abp.Notification.Web.Menus;

public class AbpNotificationMenuContributor : IMenuContributor
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
    // Add main menu items.
    context.Menu.AddItem(new ApplicationMenuItem(AbpNotificationMenuNames.Prefix, displayName: "Notification", "~/Notification", icon: "fa fa-globe"));

    return Task.CompletedTask;
  }
}
