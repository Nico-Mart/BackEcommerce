

using Application.Models.Product;

namespace Application.Interfaces
{
    public interface IUserService : IService<CreateUserDto,ReadUserDto,UpdateUserDto>
    {
    }
}
