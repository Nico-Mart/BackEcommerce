using Application.Models.Order;

namespace Application.Interfaces
{
    public interface IOrderService : IService<CreateOrderDto, ReadOrderDto, UpdateOrderDto>
    {
    }
}
