// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.UI.Navigation;

using X.Abp.FileManagement.Localization;
using X.Abp.FileManagement.Permissions;

namespace X.Abp.FileManagement.Web.Menus;

public class AbpFileManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var featureChecker = context.ServiceProvider.GetService<IFeatureChecker>();

        if (await featureChecker.IsEnabledAsync(FileManagementFeatures.Enable))
        {
            var localizer = context.GetLocalizer<FileManagementResource>();

            var fileManagementMenuItem = new ApplicationMenuItem(AbpFileManagementMenuNames.GroupName, localizer["Menu:FileManagement"], "~/FileManagement", icon: "fa fa-folder-open").RequirePermissions(AbpFileManagementPermissions.DirectoryDescriptor.Default);

            context.Menu.AddItem(fileManagementMenuItem);
        }
    }
}
