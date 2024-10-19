

namespace Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
