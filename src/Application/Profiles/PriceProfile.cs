using Application.Models.Product;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class PriceProfile : Profile
    {
        public PriceProfile()
        {
            CreateMap<PriceDto, Price>();
        }
    }
}