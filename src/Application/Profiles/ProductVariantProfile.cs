using Application.Models.ProductVariant;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
