using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;

namespace Application.Services
{
    public abstract class Service<TEntity, TReadDto, TDto> : IService<TReadDto, TDto>
        where TEntity : class
        where TReadDto : class
        where TDto : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected Service(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TReadDto> Create(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            entity = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<TReadDto>(entity);
        }

        public virtual async Task<ICollection<TReadDto>> CreateRange(ICollection<TDto> dtos)
        {
            var entities = _mapper.Map<ICollection<TEntity>>(dtos);
            await _repository.CreateRangeAsync(entities);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ICollection<TReadDto>>(entities);
        }

        public virtual async Task<ICollection<TReadDto>> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<ICollection<TReadDto>>(entities);
        }

        public virtual async Task<TReadDto> GetById<Tid>(Tid id) where Tid : notnull
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"The given key '{id}' does not correspond to a {typeof(TEntity).Name}");
            return _mapper.Map<TReadDto>(entity);
        }

        public abstract void Update(TDto dto);

        public abstract void UpdateRange(ICollection<TDto> dtos);

        public abstract void Delete(TDto dto);

        public abstract void DeleteRange(ICollection<TDto> dtos);
    }
}
