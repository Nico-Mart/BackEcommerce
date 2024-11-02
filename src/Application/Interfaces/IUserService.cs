using Application.Models.Password;
using Application.Models.Register;
using Application.Models.User;

namespace Application.Interfaces
{
    public interface IUserService : IService<CreateUserDto,ReadUserDto,UpdateUserDto>
    {
        Task ActivateAccount(string token);
        Task<ReadUserDto> CreateWithoutEmailVerification(CreateUserDto userDto);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);
        Task RequestPasswordReset(string email);
        Task<string> GenerateVerificationToken(string email);
    }
}
