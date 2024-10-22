using Application.Interfaces;
using Application.Models.Product;
using Application.Shared.Classes;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService : Service<Product, CreateProductDto, ReadProductDto, UpdateProductDto>, IProductService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPriceRepository _priceRepository;
        public ProductService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IPriceRepository priceRepository,
            IMapper mapper
        ) : base(productRepository, mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _priceRepository = priceRepository;
        }

        public override async Task<ReadProductDto> Create(CreateProductDto productDto)
        {
            //Ensure that the product has a price
            if (productDto.Price == null) throw new ArgumentNullException(nameof(productDto.Price), "A price must be provided for a product");

            //Ensure that the category exists
            bool categoryExists = await _categoryRepository.GetByIdAsync(productDto.IdCategory) != null;
            if (!categoryExists) throw new ArgumentException("Invalid category", nameof(productDto.IdCategory));

            //Process the DTO
            var readDto = await base.Create(productDto);

            //Create the price entity and assign its related product
            var price = _mapper.Map<Price>(productDto.Price);
            price.IdProduct = readDto.Id;

            //Save the price
            await _priceRepository.CreateAsync(price);
            readDto.Price = price.Value;

            return readDto;
        }

        public override async Task<ICollection<ReadProductDto>> CreateRange(ICollection<CreateProductDto> productDtos)
        {
            //Ensure that each product has a price and a valid category
            foreach (var dto in productDtos)
            {
                if (dto.Price == null) throw new ArgumentNullException(nameof(dto.Price), "A price must be provided for each product.");
                bool categoryExists = await _categoryRepository.GetByIdAsync(dto.IdCategory) != null;
                if (!categoryExists) throw new ArgumentException("Some products have an innvalid category", nameof(dto.IdCategory));
            }

            //Process the DTOs
            var readProductDtos = await base.CreateRange(productDtos);

            //Create the price entities and assign their related products
            var prices = new List<Price>();
            foreach (var (dto, readDto) in productDtos.Zip(readProductDtos, (dto, readDto) => (dto, readDto)))
            {
                var price = _mapper.Map<Price>(dto.Price);
                price.IdProduct = readDto.Id; //Assign the product ID
                prices.Add(price); //Add the price to the result list
                readDto.Price = price.Value; //Assign price to the read DTO
            }

            //Save the prices to repository
            await _priceRepository.CreateRangeAsync(prices);

            return readProductDtos;
        }

        public override async Task<ICollection<ReadProductDto>> GetAll(Options? options)
        {
            var list = await base.GetAll(options);
            foreach (var item in list)
            {
                var price = await _priceRepository.GetByIdAsync(item.Id);
                if (price != null) item.Price = price.Value;
            }
            return list;
        }

        private async Task<ICollection<Product>> GetByIds(List<int> ids)
        {
            var query = _productRepository.GetAll().Where(p => ids.Contains(p.Id));
            return await _productRepository.ToListAsync(query);
        }

        public override async Task Update(UpdateProductDto productDto)
        {
            var entity = await _productRepository.GetByIdAsync(productDto.Id);
            if (entity == null) throw new KeyNotFoundException($"The given key '{productDto.Id}' is not related to a product.");
            if (productDto.Price != null)
            {
                //Create the price entity and assign its related product
                var price = _mapper.Map<Price>(productDto.Price);
                price.IdProduct = productDto.Id;

                //Save the price
                await _priceRepository.CreateAsync(price);
            }
            entity = _mapper.Map(productDto, entity);
            await _productRepository.UpdateAsync(entity);
        }

        public override async Task<int> UpdateRange(ICollection<UpdateProductDto> productDtos)
        {
            //Fetch the entities to update
            var ids = productDtos.Select(p => p.Id).ToList();
            var query = _productRepository.GetAllWithPrices().Where(p => ids.Contains(p.Id));
            var entities = await _productRepository.ToListAsync(query);

            //Ensure entities is not empty
            if (entities == null || !entities.Any()) throw new KeyNotFoundException("No products were found with the provided keys");

            //Update entities
            foreach (var entity in entities)
            {
                var productDto = productDtos.FirstOrDefault(dto => dto.Id == entity.Id);
                if (productDto != null) _mapper.Map(productDto, entity);
            }

            //Save entities
            return await _repository.UpdateRangeAsync(entities);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var entity = await _productRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"The given key '{id}' is not related to a product.");
            await _productRepository.DeleteAsync(entity);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {
            var query = _productRepository.GetAllWithPrices().Where(p => ids.Contains((Tid)(Object)p.Id));
            var entities = await _productRepository.ToListAsync(query);
            if (entities == null || !entities.Any()) throw new KeyNotFoundException("No products were found with the provided keys");
            return await _productRepository.DeleteRangeAsync(entities);
        }
    }
}
