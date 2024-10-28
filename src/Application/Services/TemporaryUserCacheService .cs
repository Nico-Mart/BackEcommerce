using Application.Interfaces;
using Application.Models.User;
using Microsoft.Extensions.Caching.Memory;


namespace Application.Services
{
    public class TemporaryUserCacheService : ITemporaryUserCacheService
    {
        private readonly IMemoryCache _cache;

        public TemporaryUserCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void StoreTemporaryUser(string token, CreateUserDto userDto, TimeSpan expiration)
        {
            _cache.Set(token, userDto, expiration);
        }
        public void StoreTemporaryUser(string token, (string email, CreateUserDto userDto) data, TimeSpan expiration)
        {
            _cache.Set(token, data, expiration);
        }

        public CreateUserDto? GetTemporaryUserByToken(string token)
        {
            _cache.TryGetValue(token, out CreateUserDto? user);
            return user;
        }

        public (string email, CreateUserDto userDto)? GetTemporaryUserDataByToken(string token)
        {
            if (_cache.TryGetValue(token, out var userData))
            {
                return ((string email, CreateUserDto userDto)?)userData;
            }
            return null;
        }

        public void RemoveTemporaryUser(string token)
        {
            _cache.Remove(token);
        }

        public bool CheckIfUserExists(string email)
        {
            return _cache.TryGetValue($"user_{email}", out _);
        }

    }

}
