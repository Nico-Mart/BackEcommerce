
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
        Task<ICollection<TReadDto>> GetAll(Options? options);
        Task<TReadDto> GetByIdAsync(int id);
        void Update(TUpdateDto dto);
        Task<int> UpdateRange(ICollection<TUpdateDto> dtos);
        void Delete<Tid>(Tid id) where Tid : notnull;
        Task<int> DeleteRange<Tid>(List<Tid> ids) where Tid : notnull;
    }
}
