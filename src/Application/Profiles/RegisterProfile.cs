using Application.Models.Register;
using Application.Models.User;
using AutoMapper;
using Domain.Entities;


namespace Application.Profiles
{
    class RegisterProfile : Profile
    {
        public RegisterProfile()
        {
            CreateMap<CreateRegisterUserDto, CreateUserDto>();
            CreateMap<Client, UpdateUserDto>();
            CreateMap<UpdateRegisterUserDto, UpdateUserDto>();
        }
    }
}
