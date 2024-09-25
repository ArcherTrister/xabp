// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.UI.Navigation;

using X.Abp.TextTemplateManagement.Features;
using X.Abp.TextTemplateManagement.Localization;
using X.Abp.TextTemplateManagement.Permissions;

namespace X.Abp.TextTemplateManagement.Web.Menus;

public class AbpTextTemplateManagementMenuContributor : IMenuContributor
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
    IStringLocalizer localizer = context.GetLocalizer<TextTemplateManagementResource>();

    ApplicationMenuItem menuItem = new ApplicationMenuItem(
        AbpTextTemplateManagementMenuNames.GroupName,
        localizer["Menu:TextTemplates"],
        "~/TextTemplateManagement",
        "fa fa-text-width")
        .RequireFeatures(TextTemplateManagementFeatures.Enable)
        .RequirePermissions(AbpTextTemplateManagementPermissions.TextTemplates.Default);
    context.Menu.GetAdministration().AddItem(menuItem);

    return Task.CompletedTask;
  }
}
