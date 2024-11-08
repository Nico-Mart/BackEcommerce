using Application.Interfaces;
using Application.Models.Order;
using Application.Shared.Classes;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using EllipticCurve.Utils;

namespace Application.Services
{
    public class OrderService : Service<Order, CreateOrderDto, ReadOrderDto, UpdateOrderDto>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IPriceRepository priceRepository,
            IProductVariantRepository productVariantRepository,
            IMapper mapper
        ) : base(orderRepository, mapper)
        {
            _orderRepository = orderRepository;
            _priceRepository = priceRepository;
            _productVariantRepository = productVariantRepository;
        }

        public override async Task<PagedResult<ReadOrderDto>> GetAll(Options? options = null)
        {
            var result = await base.GetAll(options);

            foreach (var order in result.Data)
            {
                foreach (var line in order.OrderLines)
                {
                    var price = await _priceRepository.GetByIdAsync(line.ProductVariant.IdProduct);
                    line.Price = price?.Value ?? -1;
                }
            }
            
            return result;
        }

        public override async Task<ReadOrderDto> Create(CreateOrderDto createOrderDto)
        {
            //Check variants existence and availability
            var nonExistentPVariants = new List<int>();
            var insufficientStocks = new List<int>();
            foreach (var productDto in createOrderDto.OrderLines!)
            {
                var productVariant = await _productVariantRepository.GetByIdAsync(productDto.IdProductVariant);
                if (productVariant == null)
                {
                    nonExistentPVariants.Add(productDto.IdProductVariant);
                    continue;
                }

                if (productVariant.Stock < productDto.Amount)
                    insufficientStocks.Add(productDto.IdProductVariant);
            }

            if (insufficientStocks.Count != 0)
                throw new InvalidOperationException($"Not enough stock for product variants with IDs: {insufficientStocks}.");
            if (nonExistentPVariants.Count != 0)
                throw new KeyNotFoundException($"Product variants with IDs {nonExistentPVariants} not found.");

            var readDto = await base.Create(createOrderDto);
            foreach (var line in readDto.OrderLines)
            {
                var price = await _priceRepository.GetByIdAsync(line.ProductVariant.IdProduct);
                line.Price = price?.Value ?? -1;
            }
            return readDto;
        }

        #region Obsolete Methods
        [Obsolete("This method should not be used")]
        public override Task Update(UpdateOrderDto dto)
        {
            throw new NotImplementedException();
        }

        [Obsolete("This method should not be used")]
        public override Task<int> UpdateRange(ICollection<UpdateOrderDto> dtos)
        {
            throw new NotImplementedException();
        }

        [Obsolete("This method should not be used")]
        public override Task Delete<Tid>(Tid id)
        {
            throw new NotImplementedException();
        }

        [Obsolete("This method should not be used")]
        public override Task<int> DeleteRange<Tid>(List<Tid> ids)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}