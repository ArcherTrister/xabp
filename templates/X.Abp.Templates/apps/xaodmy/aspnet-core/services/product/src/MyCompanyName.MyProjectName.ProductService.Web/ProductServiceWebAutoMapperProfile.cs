using AutoMapper;
using MyCompanyName.MyProjectName.ProductService.Products;

namespace MyCompanyName.MyProjectName.ProductService.Web;

public class ProductServiceWebAutoMapperProfile : Profile
{
    public ProductServiceWebAutoMapperProfile()
    {
        CreateMap<ProductDto, ProductUpdateDto>();
    }
}
