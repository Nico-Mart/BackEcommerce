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
                if (options?.FilterGroup.Filters.Count > 0)
                {
                    query = ApplyFilter(query, options.FilterGroup);
                }
                if (options?.Sorter != null)
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
        private IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, FilterGroup filterGroup)
        {
            if (filterGroup.Filters.Count <= 0 && filterGroup.ChildGroups.Count <= 0) return query;

            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var combinedExpression = CreateGroupExpression(parameter, filterGroup);

            return query.Where(Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter));
        }

        private Expression CreateGroupExpression(ParameterExpression parameter, FilterGroup group)
        {
            Expression? combinedExpression = null;

            foreach (var filter in group.Filters)
            {
                var predicate = CreateExpression(parameter, filter);
                combinedExpression = combinedExpression == null
                    ? predicate
                    : (group.LogicalOperator == LogicalOperator.And
                        ? Expression.AndAlso(combinedExpression, predicate)
                        : Expression.OrElse(combinedExpression, predicate));
            }

            //Calls recursively until there's no more childGroups
            foreach (var childGroup in group.ChildGroups)
            {
                var childPredicate = CreateGroupExpression(parameter, childGroup);
                combinedExpression = combinedExpression == null
                    ? childPredicate
                    : (group.LogicalOperator == LogicalOperator.And
                        ? Expression.AndAlso(combinedExpression, childPredicate)
                        : Expression.OrElse(combinedExpression, childPredicate));
            }

            return combinedExpression!;
        }

        private Expression CreateExpression(ParameterExpression parameter, Filter filter)
        {
            var property = Expression.Property(parameter, filter.Field);
            var parsedValue = Convert.ChangeType(filter.Value, property.Type);
            var searchValue = Expression.Constant(parsedValue, property.Type);

            return filter.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(property, searchValue),
                FilterOperator.NotEquals => Expression.NotEqual(property, searchValue),
                FilterOperator.GreaterThan => Expression.GreaterThan(property, searchValue),
                FilterOperator.LessThan => Expression.LessThan(property, searchValue),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, searchValue),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, searchValue),
                FilterOperator.Contains => Expression.Call(property, "Contains", null, searchValue),
                FilterOperator.NotContains => Expression.Not(Expression.Call(property, "Contains", null, searchValue)),
                FilterOperator.BeginsWith => Expression.Call(property, "StartsWith", null, searchValue),
                FilterOperator.NotBeginsWith => Expression.Not(Expression.Call(property, "StartsWith", null, searchValue)),
                FilterOperator.EndsWith => Expression.Call(property, "EndsWith", null, searchValue),
                FilterOperator.NotEndsWith => Expression.Not(Expression.Call(property, "EndsWith", null, searchValue)),
                _ => throw new NotSupportedException($"Unsupported filter operator: {filter.Operator}")
            };
        }

        private IQueryable<TEntity> ApplySorter(IQueryable<TEntity> query, Sorter sorter)
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
