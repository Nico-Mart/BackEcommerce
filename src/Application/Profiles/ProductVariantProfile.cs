using Application.Models.ProductVariant;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class ProductVariantProfile : Profile
    {
        public ProductVariantProfile()
        {
            CreateMap<ProductVariantDto, ProductVariant>();
            CreateMap<ProductVariant, ReadProductVariantDto>();
        }
    }
}
