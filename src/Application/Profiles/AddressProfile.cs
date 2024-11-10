using Application.Models.Address;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<CreateAddressDto, Address>();
            CreateMap<Address, ReadAddressDto>();
        }
    }
}
