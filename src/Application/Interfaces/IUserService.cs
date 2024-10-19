using Application.Models.User;

namespace Application.Interfaces
{
    public interface IUserService : IService<CreateUserDto,ReadUserDto,UpdateUserDto>
    {
        Task ActivateUser(int userId);
        Task<string> GenerateVerificationToken(int userId);
    }
}
