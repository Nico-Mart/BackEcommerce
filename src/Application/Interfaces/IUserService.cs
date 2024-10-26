using Application.Models.User;

namespace Application.Interfaces
{
    public interface IUserService : IService<CreateUserDto,ReadUserDto,UpdateUserDto>
    {
        Task ActivateAccount(string token);
        Task<string> GenerateVerificationToken(string email);
    }
}
