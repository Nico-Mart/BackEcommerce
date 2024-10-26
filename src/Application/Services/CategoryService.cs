using Application.Interfaces;
<<<<<<< Updated upstream
using Application.Models.ProductMisc;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
=======
>>>>>>> Stashed changes

namespace Application.Services
{
    public class CategoryService : Service<Category, CreateCategoryDto, CategoryDto, CategoryDto>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : base(categoryRepository, mapper)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task Update(CategoryDto categoryDto)
        {
            var entity = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (entity == null) throw new KeyNotFoundException($"The given key '{categoryDto.Id}' is not related to a category.");
            
            entity = _mapper.Map(categoryDto, entity);
            await _categoryRepository.UpdateAsync(entity);
        }

        public override async Task<int> UpdateRange(ICollection<CategoryDto> categoryDtos)
        {
            //Fetch the entities to update
            var ids = categoryDtos.Select(c => c.Id).ToList();
            var query = _categoryRepository.GetAll().Where(c => ids.Contains(c.Id));
            var entities = await _categoryRepository.ToListAsync(query);

            //Ensure entities is not empty
            if (entities == null || !entities.Any()) throw new KeyNotFoundException("No categories were found with the provided keys");

            //Update entities
            foreach (var entity in entities)
            {
                var categoryDto = categoryDtos.FirstOrDefault(dto => dto.Id == entity.Id);
                if (categoryDto != null) _mapper.Map(categoryDto, entity);
            }

            //Save entities
            return await _repository.UpdateRangeAsync(entities);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"The given key '{id}' is not related to a category.");
            await _categoryRepository.DeleteAsync(entity);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {
            var query = _categoryRepository.GetAll().Where(p => ids.Contains((Tid)(Object)p.Id));
            var entities = await _categoryRepository.ToListAsync(query);
            if (entities == null || !entities.Any()) throw new KeyNotFoundException("No categories were found with the provided keys");
            return await _categoryRepository.DeleteRangeAsync(entities);
        }
    }
}
