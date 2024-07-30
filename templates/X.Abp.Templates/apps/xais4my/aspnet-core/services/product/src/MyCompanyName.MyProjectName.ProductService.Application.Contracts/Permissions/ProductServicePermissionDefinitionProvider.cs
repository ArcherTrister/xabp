using MyCompanyName.MyProjectName.ProductService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyCompanyName.MyProjectName.ProductService.Permissions;

public class ProductServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ProductServicePermissions.GroupName, L("Permission:ProductService"));

        var productPermission = myGroup.AddPermission(ProductServicePermissions.Products.Default, L("Permission:Products"));
        productPermission.AddChild(ProductServicePermissions.Products.Create, L("Permission:Create"));
        productPermission.AddChild(ProductServicePermissions.Products.Edit, L("Permission:Edit"));
        productPermission.AddChild(ProductServicePermissions.Products.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ProductServiceResource>(name);
    }
}
