using System.Collections.Generic;
using System.Threading.Tasks;
using MyCompanyName.MyProjectName.ProductService.Localization;
using MyCompanyName.MyProjectName.ProductService.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace MyCompanyName.MyProjectName.ProductService.Web.Menus;

public class ProductServiceMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return;
        }

        var moduleMenu = AddModuleMenuItem(context);
        AddMenuItemProducts(context, moduleMenu);
    }

    private static ApplicationMenuItem AddModuleMenuItem(MenuConfigurationContext context)
    {
        var moduleMenu = new ApplicationMenuItem(
            ProductServiceMenus.Prefix,
            context.GetLocalizer<ProductServiceResource>()["Menu:ProductService"],
            icon: "fa fa-folder"
        );

        context.Menu.Items.AddIfNotContains(moduleMenu);
        return moduleMenu;
    }
        
    private static void AddMenuItemProducts(MenuConfigurationContext context, ApplicationMenuItem parentMenu)
    {
        parentMenu.AddItem(
            new ApplicationMenuItem(
                ProductServiceMenus.Products,
                context.GetLocalizer<ProductServiceResource>()["Menu:Products"],
                url: "/Products"
            ).RequirePermissions(ProductServicePermissions.Products.Default));

        context.Menu.Items.AddIfNotContains(parentMenu);
    }
}
