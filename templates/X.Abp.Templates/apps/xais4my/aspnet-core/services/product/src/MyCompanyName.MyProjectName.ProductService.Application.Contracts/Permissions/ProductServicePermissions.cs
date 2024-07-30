using Volo.Abp.Reflection;

namespace MyCompanyName.MyProjectName.ProductService.Permissions;

public class ProductServicePermissions
{
    public const string GroupName = "ProductService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ProductServicePermissions));
    }

    public class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
