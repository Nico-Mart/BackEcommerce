using Application.Interfaces;
using Application.Models.Product;
using Application.Models.ProductVariant;
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
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IPriceRepository _priceRepository;
        public ProductService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IPriceRepository priceRepository,
            IMapper mapper
        ) : base(productRepository, mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _priceRepository = priceRepository;
        }

        public override async Task<ReadProductDto> Create(CreateProductDto productDto)
        {
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
            //Ensure that each product has a valid category
            foreach (var dto in productDtos)
            {
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

        public override async Task<PagedResult<ReadProductDto>> GetAll(Options? options = null)
        {
            var list = await base.GetAll(options);
            foreach (var item in list.Data)
            {
                var price = await _priceRepository.GetByIdAsync(item.Id);
                if (price != null) item.Price = price.Value;
            }
            return list;
        }

        public override async Task Update(UpdateProductDto productDto)
        {
            //Fetch the entity to update with its related productVariants
            var entity = await _productRepository.GetByIdAsync(productDto.Id)
                ?? throw new KeyNotFoundException($"The given key '{productDto.Id}' is not related to a product.");

            if (productDto.Price != null)
            {
                //Create the price entity and assign its related product
                var price = _mapper.Map<Price>(productDto.Price);
                price.IdProduct = productDto.Id;

                //Save the price
                await _priceRepository.CreateAsync(price);
            }
            if (productDto.ProductVariants != null)
                await _updateProductVariants(entity.ProductVariants, productDto.ProductVariants, entity.Id);

            entity = _mapper.Map(productDto, entity);
            await _productRepository.UpdateAsync(entity);
        }

        private async Task _updateProductVariants(ICollection<ProductVariant> existingVariants, ICollection<UpdateProductVariantDto> dtoVariants, int dtoId)
        {
            var variantsToDelete = dtoVariants.Count == 0
                ? [.. existingVariants]
                : existingVariants
                    .Where(ev => !dtoVariants.Any(dv => dv.Id == ev.Id))
                    .ToList();
            var variantsToCreate = dtoVariants?
                .Where(dv => dv.Id == 0)
                .Select(dv => _mapper.Map<ProductVariant>(dv))
                .ToList();
            var variantsToUpdate = dtoVariants?
                .Where(dv => existingVariants.Any(ev => ev.Id == dv.Id))
                .ToList();

            //1. Delete variants that are not in the DTO but exist in the database
            if (variantsToDelete.Count != 0)
            {
                await _productVariantRepository.DeleteRangeAsync(variantsToDelete);
            }

            //2. Update variants that exist in both the DTO and the database
            if (variantsToUpdate != null && variantsToUpdate.Count != 0)
            {
                foreach (var variantDto in variantsToUpdate)
                {
                    var existingVariant = existingVariants.First(v => v.Id == variantDto.Id);
                    _mapper.Map(variantDto, existingVariant);
                    await _productVariantRepository.UpdateAsync(existingVariant);
                }
            }

            //3. Create new variants that are in the DTO but not in the database
            if (variantsToCreate != null && variantsToCreate.Count != 0)
            {
                foreach (var variant in variantsToCreate)
                {
                    variant.IdProduct = dtoId; //Assign product ID
                    await _productVariantRepository.CreateAsync(variant);
                }
            }
        }

        public override async Task<int> UpdateRange(ICollection<UpdateProductDto> productDtos)
        {
            //Fetch the entities to update with their related productVariants
            var ids = productDtos.Select(p => p.Id).ToList();
            var query = _productRepository.GetAll().Where(p => ids.Contains(p.Id));
            var entities = await _productRepository.ToListAsync(query);

            //Ensure entities is not empty
            if (entities == null || entities.Count == 0) throw new KeyNotFoundException("No products were found with the provided keys");

            //Update entities
            foreach (var entity in entities)
            {
                var productDto = productDtos.FirstOrDefault(dto => dto.Id == entity.Id);
                if (productDto != null)
                {
                    if (productDto.ProductVariants != null)
                        await _updateProductVariants(entity.ProductVariants, productDto.ProductVariants, entity.Id);
                    _mapper.Map(productDto, entity);
                }
            }

            //Save entities
            return await _repository.UpdateRangeAsync(entities);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var entity = await _productRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"The given key '{id}' is not related to a product.");
            await _productRepository.DeleteAsync(entity);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {
            var query = _productRepository.GetAllWithPrices().Where(p => ids.Contains((Tid)(Object)p.Id));
            var entities = await _productRepository.ToListAsync(query);
            if (entities == null || entities.Count == 0) throw new KeyNotFoundException("No products were found with the provided keys");
            return await _productRepository.DeleteRangeAsync(entities);
        }
    }
    public class ProductVariantService : Service<ProductVariant, CreateProductVariantDto, ReadProductVariantDto, UpdateProductVariantDto>, IProductVariantService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        public ProductVariantService(
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IMapper mapper
        ) : base(productVariantRepository, mapper)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<ReadProductVariantDto> Create(int productId, CreateProductVariantDto productVariantDto)
        {
            _ = _productRepository.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException($"The given key '{productId}' is not related to a product.");
            return await base.Create(productVariantDto);
        }

        public async Task<ICollection<ReadProductVariantDto>> CreateRange(int productId, ICollection<CreateProductVariantDto> productVariantDtos)
        {
            _ = _productRepository.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException($"The given key '{productId}' is not related to a product.");
            return await base.CreateRange(productVariantDtos);
        }

        public async Task<ICollection<ReadProductVariantDto>> GetAll(int id)
        {
            var query = _productVariantRepository.GetAll().Where(pv => pv.IdProduct == id);
            var list = await _repository.ToListAsync(query);
            if (list == null || list.Count == 0) throw new KeyNotFoundException($"The given key '{id}' is not related to a product.");
            return _mapper.Map<ICollection<ReadProductVariantDto>>(list);

        }

        public override async Task Update(UpdateProductVariantDto productVariantDto)
        {
            var entity = await _productVariantRepository.GetByIdAsync(productVariantDto.Id)
                ?? throw new KeyNotFoundException($"The given key '{productVariantDto.Id}' is not related to a productVariant.");
            _ = await _productRepository.GetByIdAsync(productVariantDto.IdProduct)
                ?? throw new KeyNotFoundException($"The given key '{productVariantDto.IdProduct}' is not related to a product.");
            entity = _mapper.Map(productVariantDto, entity);
            await _productVariantRepository.UpdateAsync(entity);
        }

        public override async Task<int> UpdateRange(ICollection<UpdateProductVariantDto> productVariantDtos)
        {
            //Fetch the entities to update
            var ids = productVariantDtos.Select(p => p.Id).ToList();
            var query = _productVariantRepository.GetAll().Where(p => ids.Contains(p.Id));
            var entities = await _productVariantRepository.ToListAsync(query);

            //Ensure entities is not empty
            if (entities == null || entities.Count == 0) throw new KeyNotFoundException("No products were found with the provided keys");

            //Update entities
            foreach (var entity in entities)
            {
                var product = await _productRepository.GetByIdAsync(entity.IdProduct);
                if (product == null) continue;
                var productVariantDto = productVariantDtos.FirstOrDefault(dto => dto.Id == entity.Id);
                if (productVariantDto != null) _mapper.Map(productVariantDto, entity);
            }

            //Save entities
            return await _repository.UpdateRangeAsync(entities);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var entity = await _productVariantRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"The given key '{id}' is not related to a productVariant.");
            await _productVariantRepository.DeleteAsync(entity);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {
            var query = _productVariantRepository.GetAll().Where(p => ids.Contains((Tid)(Object)p.Id));
            var entities = await _productVariantRepository.ToListAsync(query);
            if (entities == null || entities.Count == 0) throw new KeyNotFoundException("No productVariants were found with the provided keys");
            return await _productVariantRepository.DeleteRangeAsync(entities);
        }

        #region Obsolete Methods
        [Obsolete("This method should not be used")]
        public override Task<ReadProductVariantDto> Create(CreateProductVariantDto dto)
        {
            throw new NotImplementedException();
        }

        [Obsolete("This method should not be used")]
        public override Task<ICollection<ReadProductVariantDto>> CreateRange(ICollection<CreateProductVariantDto> dtos)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
