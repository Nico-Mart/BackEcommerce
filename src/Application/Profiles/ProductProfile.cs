using Application.Models.Product;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDto, Product>()
                .ForMember(
                    dest => dest.ProductVariants,
                    opt => opt.MapFrom(src => src.ProductVariants));

            CreateMap<Product, ReadProductDto>()
                .ForMember(
                    dest => dest.ProductVariants,
                    opt => opt.MapFrom(src => src.ProductVariants));

            CreateMap<UpdateProductDto, Product>()
                .ForMember(
                    dest => dest.ProductVariants,
                    opt => opt.Ignore());
        }
    }
}
