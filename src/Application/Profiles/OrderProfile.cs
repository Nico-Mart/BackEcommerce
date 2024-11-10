using Application.Models.Order;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderDto, Order>()
                .ForMember(
                    dest => dest.OrderLines,
                    opt => opt.MapFrom(src => src.OrderLines))
                .ForMember(
                    dest => dest.IdAddressNavigation,
                    opt => opt.MapFrom(src => src.Address));

            CreateMap<Order, ReadOrderDto>()
                .ForMember(
                    dest => dest.OrderLines,
                    opt => opt.MapFrom(src => src.OrderLines))
                .ForMember(
                    dest => dest.Address,
                    opt => opt.MapFrom(src => src.IdAddressNavigation));

            CreateMap<UpdateOrderDto, Order>()
                .ForMember(
                    dest => dest.OrderLines,
                    opt => opt.Ignore());

            CreateMap<OrderLineDto, OrderLine>();

            CreateMap<OrderLine, ReadOrderLineDto>()
                .ForMember(
                    dest => dest.ProductVariant,
                    opt => opt.MapFrom(src => src.IdProductVariantNavigation)
                );

            CreateMap<ProductVariant, OrderLineProductVariant>();
        }
    }
}