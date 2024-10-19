using Application.Interfaces;
using System.Security.Cryptography;

namespace Application.Services
{

    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, 16, 10000))
            {
                var salt = rfc2898.Salt;
                var hash = rfc2898.GetBytes(32);

                var result = new byte[48];
                Buffer.BlockCopy(salt, 0, result, 0, 16);
                Buffer.BlockCopy(hash, 0, result, 16, 32);

                return Convert.ToBase64String(result);
            }
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hashedBytes = Convert.FromBase64String(hashedPassword);

            var salt = new byte[16];
            Buffer.BlockCopy(hashedBytes, 0, salt, 0, 16);

            using (var rfc2898 = new Rfc2898DeriveBytes(providedPassword, salt, 10000))
            {
                var hash = rfc2898.GetBytes(32);
                for (int i = 0; i < 32; i++)
                {
                    if (hashedBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
