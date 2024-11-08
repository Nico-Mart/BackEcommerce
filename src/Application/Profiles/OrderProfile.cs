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
                    opt => opt.MapFrom(src => src.OrderLines));

            CreateMap<Order, ReadOrderDto>()
                .ForMember(
                    dest => dest.OrderLines,
                    opt => opt.MapFrom(src => src.OrderLines));

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