using Application.Models.Product;
using AutoMapper;
using Domain.Entities;


namespace Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<ReadUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
