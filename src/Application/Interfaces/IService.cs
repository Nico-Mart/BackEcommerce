
namespace Application.Interfaces
{
    public interface IService<TReadDto, TDto> where TReadDto : class where TDto : class
    {
        Task<TReadDto> Create(TDto product);
        Task<ICollection<TReadDto>> CreateRange(ICollection<TDto> products);
        Task<ICollection<TReadDto>> GetAll();
        Task<TReadDto> GetById<Tid>(Tid id) where Tid : notnull;
        void Update(TDto product);
        void UpdateRange(ICollection<TDto> products);
        void Delete(TDto product);
        void DeleteRange(ICollection<TDto> products);
    }
}
