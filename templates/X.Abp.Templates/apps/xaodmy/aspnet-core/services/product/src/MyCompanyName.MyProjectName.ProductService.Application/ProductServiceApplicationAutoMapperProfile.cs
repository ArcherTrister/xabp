using AutoMapper;
using MyCompanyName.MyProjectName.ProductService.Products;

namespace MyCompanyName.MyProjectName.ProductService;

public class ProductServiceApplicationAutoMapperProfile : Profile
{
    public ProductServiceApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}
