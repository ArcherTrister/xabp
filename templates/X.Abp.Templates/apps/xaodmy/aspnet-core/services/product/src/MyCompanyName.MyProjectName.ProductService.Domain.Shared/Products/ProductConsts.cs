namespace MyCompanyName.MyProjectName.ProductService.Products;

public static class ProductConsts
{
    public const int NameMaxLength = 64;

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Product." : string.Empty);
    }

    private const string DefaultSorting = "{0}Name asc";
}
