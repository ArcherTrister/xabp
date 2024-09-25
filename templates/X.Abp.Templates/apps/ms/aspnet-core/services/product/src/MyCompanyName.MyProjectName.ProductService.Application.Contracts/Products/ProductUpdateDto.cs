using System.ComponentModel.DataAnnotations;

namespace MyCompanyName.MyProjectName.ProductService.Products;

public class ProductUpdateDto
{
    [Required]
    [StringLength(ProductConsts.NameMaxLength)]
    public string Name { get; set; }

    [Required]
    public float Price { get; set; }
}
