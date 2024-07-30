namespace MyCompanyName.MyProjectName.ProductService;

public static class ProductServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "ProductService";
}
