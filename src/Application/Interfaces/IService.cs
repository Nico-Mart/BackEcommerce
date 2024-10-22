using Application.Shared.Classes;

namespace Application.Interfaces
{
    public interface IService<TCreateDto, TReadDto, TUpdateDto>
        where TCreateDto : class
        where TReadDto : class
        where TUpdateDto : class
    {
        Task<TReadDto> Create(TCreateDto dto);
        Task<ICollection<TReadDto>> CreateRange(ICollection<TCreateDto> dtos);
        Task<PagedResult<TReadDto>> GetAll(Options? options = null);
        Task<TReadDto> GetByIdAsync<Tid>(Tid id) where Tid : notnull;
        Task Update(TUpdateDto dto);
        Task<int> UpdateRange(ICollection<TUpdateDto> dtos);
        Task Delete<Tid>(Tid id) where Tid : notnull;
        Task<int> DeleteRange<Tid>(List<Tid> ids) where Tid : notnull;
    }
}
