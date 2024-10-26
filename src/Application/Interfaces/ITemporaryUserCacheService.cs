using Application.Models.User;

namespace Application.Interfaces
{
    public interface ITemporaryUserCacheService
    {
        void StoreTemporaryUser(string token, CreateUserDto user, TimeSpan expiration);
        CreateUserDto? GetTemporaryUserByToken(string token);
        void RemoveTemporaryUser(string token);
        bool CheckIfUserExists(string email);
    }

}
