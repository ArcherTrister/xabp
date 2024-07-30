namespace MyCompanyName.MyProjectName.ProductService.Web.Pages.Products;

public class IndexModel : ProductServicePageModel
{
    public string NameFilter { get; set; }
    public float? PriceFilterMin { get; set; }
    public float? PriceFilterMax { get; set; }
}
