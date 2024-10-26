using Application.Models.Register;
using Application.Models.User;
using AutoMapper;


namespace Application.Profiles
{
    class RegisterProfile : Profile
    {
        public RegisterProfile()
        {
            CreateMap<CreateRegisterUserDto, CreateUserDto>();
        }
    }
}
