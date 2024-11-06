using Application.Models.User;

namespace Application.Interfaces
{
    public interface ITemporaryUserCacheService
    {
        void StoreTemporaryUser(string token, CreateUserDto user, TimeSpan expiration);
        void StoreTemporaryUser(string token, (string email, CreateUserDto userDto) data, TimeSpan expiration);
        CreateUserDto? GetTemporaryUserByToken(string token);
        (string email, CreateUserDto userDto)? GetTemporaryUserDataByToken(string token);
        void RemoveTemporaryUser(string token);
        bool CheckIfUserExists(string email);
    }

}
