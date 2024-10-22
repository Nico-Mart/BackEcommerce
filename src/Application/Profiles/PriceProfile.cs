using Application.Models.ProductMisc;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class PriceProfile : Profile
    {
        public PriceProfile()
        {
            CreateMap<CreatePriceDto, Price>();
            CreateMap<PriceDto, Price>();
            CreateMap<Price, PriceDto>();
        }
    }
}