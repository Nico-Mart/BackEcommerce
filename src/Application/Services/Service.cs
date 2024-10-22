using Application.Interfaces;
using Application.Shared.Classes;
using Application.Shared.Enums;
using AutoMapper;
using Domain.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Services
{
    public abstract class Service<TEntity, TCreateDto, TReadDto, TUpdateDto> : IService<TCreateDto, TReadDto, TUpdateDto>
        where TEntity : class
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected Service(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TReadDto> Create(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            entity = await _repository.CreateAsync(entity);
            return _mapper.Map<TReadDto>(entity);
        }
        public virtual async Task<ICollection<TReadDto>> CreateRange(ICollection<TCreateDto> dtos)
        {
            var entities = _mapper.Map<ICollection<TEntity>>(dtos);
            await _repository.CreateRangeAsync(entities);
            return _mapper.Map<ICollection<TReadDto>>(entities);
        }
        public virtual async Task<PagedResult<TReadDto>> GetAll(Options? options = null)
        {
            var query = _repository.GetAll();

            if (options != null)
            {
                if (options.Filter != null)
                {
                    if (options.Filter.SearchCriteria?.Count > 0) query = ApplyFilter(query, options.Filter);
                }
                if (options.Sorter != null)
                {
                    query = ApplySorter(query, options.Sorter);
                }
            }

            var rowCount = query.Count();
            var pageCount = (int)Math.Ceiling(rowCount / (double)(options?.Paginator?.PageSize ?? 1));

            if (options?.Paginator != null)
            {
                query = query.Skip((options.Paginator.Page - 1) * options.Paginator.PageSize)
                             .Take(options.Paginator.PageSize);
            }

            var entities = await _repository.ToListAsync(query);
            var data = _mapper.Map<ICollection<TReadDto>>(entities);
            return new PagedResult<TReadDto>(data, pageCount);
        }
        public virtual async Task<TReadDto> GetByIdAsync<Tid>(Tid id) where Tid : notnull
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"No entity found with the given Id: {id}");
            return _mapper.Map<TReadDto>(entity);
        }

        #region Abstract Methods
        public abstract Task Update(TUpdateDto dto);

        public abstract Task<int> UpdateRange(ICollection<TUpdateDto> dtos);

        public abstract Task Delete<Tid>(Tid id) where Tid : notnull;

        public abstract Task<int> DeleteRange<Tid>(List<Tid> ids) where Tid : notnull;
        #endregion

        #region Custom Functions
        protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, Filter filter)
        {
            if (filter.SearchCriteria == null || filter.SearchCriteria.Count <= 0) return query;
            foreach (SearchCriteria criteria in filter.SearchCriteria)
            {
                //Convert to Expressions
                ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e"); //Entity
                MemberExpression property = Expression.Property(parameter, criteria.Field); //Entity property to filter by
                ConstantExpression searchValue = Expression.Constant(criteria.Value, typeof(string)); //Value to filter by

                Expression? predicate = null;

                switch (criteria.Operator)
                {
                    case FilterOperator.Equals:
                        predicate = Expression.Equal(property, searchValue);
                        break;
                    case FilterOperator.NotEquals:
                        predicate = Expression.NotEqual(property, searchValue);
                        break;
                    case FilterOperator.GreaterThan:
                        predicate = Expression.GreaterThan(property, Expression.Convert(searchValue, property.Type));
                        break;
                    case FilterOperator.LessThan:
                        predicate = Expression.LessThan(property, Expression.Convert(searchValue, property.Type));
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        predicate = Expression.GreaterThanOrEqual(property, Expression.Convert(searchValue, property.Type));
                        break;
                    case FilterOperator.LessThanOrEqual:
                        predicate = Expression.LessThanOrEqual(property, Expression.Convert(searchValue, property.Type));
                        break;
                    case FilterOperator.Contains:
                        predicate = Expression.Call(property, "Contains", null, searchValue);
                        break;
                    case FilterOperator.NotContains:
                        predicate = Expression.Not(Expression.Call(property, "Contains", null, searchValue));
                        break;
                    case FilterOperator.BeginsWith:
                        predicate = Expression.Call(property, "StartsWith", null, searchValue);
                        break;
                    case FilterOperator.NotBeginsWith:
                        predicate = Expression.Not(Expression.Call(property, "StartsWith", null, searchValue));
                        break;
                    case FilterOperator.EndsWith:
                        predicate = Expression.Call(property, "EndsWith", null, searchValue);
                        break;
                    case FilterOperator.NotEndsWith:
                        predicate = Expression.Not(Expression.Call(property, "EndsWith", null, searchValue));
                        break;
                }

                //Apply the criteria
                if (predicate != null) query = query.Where(Expression.Lambda<Func<TEntity, bool>>(predicate, parameter));
            }

            return query;
        }
        protected virtual IQueryable<TEntity> ApplySorter(IQueryable<TEntity> query, Sorter sorter)
        {
            //Convert to Expressions
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e"); //Entity
            MemberExpression property = Expression.Property(parameter, sorter.SortBy); //Entity property to sort by
            LambdaExpression orderByExp = Expression.Lambda(property, parameter); //Lambda function to order by parameter.property

            //Use reflection to make OrderBy Method
            var orderMethod = sorter.IsDescending ? "OrderByDescending" : "OrderBy";
            MethodInfo orderByMethodInfo = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == orderMethod && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), property.Type);

            //Invoke the OrderBy Method and return the sorted query
            return orderByMethodInfo.Invoke(null, [query, orderByExp]) as IQueryable<TEntity> ?? query;
        }
        #endregion
    }
}
