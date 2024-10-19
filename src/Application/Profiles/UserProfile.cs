using Application.Models.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;


namespace Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<CreateUserDto, Admin>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Admin));

            CreateMap<CreateUserDto, Client>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Client));

            CreateMap<CreateUserDto, Sysadmin>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Sysadmin));


            CreateMap<CreateUserDto, User>()
                .ConvertUsing((src, dest, context) =>
                {
                    switch (src.Role)
                    {
                        case Role.Admin:
                            return context.Mapper.Map<Admin>(src);
                        case Role.Client:
                            return context.Mapper.Map<Client>(src);
                        case Role.Sysadmin:
                            return context.Mapper.Map<Sysadmin>(src);
                        default:
                            throw new ArgumentException("Invalid role type");
                    }
                });

            CreateMap<User, ReadUserDto>();


            CreateMap<UpdateUserDto, Admin>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Admin));

            CreateMap<UpdateUserDto, Client>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Client));

            CreateMap<UpdateUserDto, Sysadmin>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Sysadmin));

            CreateMap<UpdateUserDto, User>()
                .ConvertUsing((src, dest, context) =>
                {
                    switch (src.Role)
                    {
                        case Role.Admin:
                            return context.Mapper.Map<Admin>(src);
                        case Role.Client:
                            return context.Mapper.Map<Client>(src);
                        case Role.Sysadmin:
                            return context.Mapper.Map<Sysadmin>(src);
                        default:
                            throw new ArgumentException("Invalid role type");
                    }
                });
        }
    }
}
