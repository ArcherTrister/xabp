// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.UI.Navigation;

using X.Abp.LanguageManagement.Localization;
using X.Abp.LanguageManagement.Permissions;

namespace X.Abp.LanguageManagement.Web.Menus;

public class AbpLanguageManagementMenuContributor : IMenuContributor
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
    IStringLocalizer localizer = context.GetLocalizer<LanguageManagementResource>();

    ApplicationMenuItem menuItem = new ApplicationMenuItem(AbpLanguageManagementMenusNames.GroupName, localizer["Menu:Languages"], icon: "fa fa-globe");
    context.Menu.GetAdministration().AddItem(menuItem);
    menuItem.AddItem(new ApplicationMenuItem(AbpLanguageManagementMenusNames.Languages, localizer["Languages"], "~/LanguageManagement")
        .RequireFeatures(LanguageManagementFeatures.Enable)
        .RequirePermissions(AbpLanguageManagementPermissions.Languages.Default));
    menuItem.AddItem(new ApplicationMenuItem(AbpLanguageManagementMenusNames.Texts, localizer["LanguageTexts"], "~/LanguageManagement/Texts")
        .RequireFeatures(LanguageManagementFeatures.Enable)
        .RequirePermissions(AbpLanguageManagementPermissions.LanguageTexts.Default));

    return Task.CompletedTask;
  }
}
