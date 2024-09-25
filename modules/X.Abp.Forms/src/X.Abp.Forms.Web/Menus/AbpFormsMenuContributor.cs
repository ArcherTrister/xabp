// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.UI.Navigation;

using X.Abp.Forms.Localization;
using X.Abp.Forms.Permissions;

namespace X.Abp.Forms.Web.Menus;

public class AbpFormsMenuContributor : IMenuContributor
{
  public virtual async Task ConfigureMenuAsync(MenuConfigurationContext context)
  {
    if (context.Menu.Name == StandardMenus.Main)
    {
      await ConfigureMainMenuAsync(context);
    }
  }

  private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
  {
    if (await context.IsGrantedAsync(AbpFormsPermissions.Forms.Default))
    {
      var localizer = context.GetLocalizer<FormsResource>();

      var formMenuItem = new ApplicationMenuItem(AbpFormsMenus.GroupName, localizer["Menu:Forms"], icon: "fa fa-list", url: "/Forms");
      context.Menu.AddItem(formMenuItem);
    }
  }
}
