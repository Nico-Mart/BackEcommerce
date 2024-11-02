using Application.Interfaces;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AutenticacionService : ICustomAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly AutenticacionServiceOptions _options;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AutenticacionService(
            IUserRepository userRepository,
            IOptions<AutenticacionServiceOptions> options,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _options = options.Value;
            _passwordHasher = passwordHasher;
        }

        private async Task<User?> ValidateUserAsync(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrEmpty(authenticationRequest.Email) || string.IsNullOrEmpty(authenticationRequest.Password))
            {
                Console.WriteLine("Email o contraseña vacíos");
                return null;
            }

            var user = await _userRepository.GetByEmailAsync(authenticationRequest.Email);
            if (user == null)
            {
                Console.WriteLine("Usuario no encontrado");
                return null;
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, authenticationRequest.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                Console.WriteLine("Contraseña inválida");
                return null;
            }

            if (user.Role.ToString() != "Admin" && user.Role.ToString() != "SysAdmin" && user.Role.ToString() != "Client")
            {
                Console.WriteLine("Rol de usuario no permitido");
                return null;
            }

            return user;
        }

        public async Task<string> AutenticarAsync(AuthenticationRequest authenticationRequest)
        {
            var user = await ValidateUserAsync(authenticationRequest);

            if (user == null)
            {
                throw new NotAllowedException("User authentication failed");
            }

            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretForKey));
            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("Email", user.Email),
                new Claim("role", user.Role.ToString())
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                credentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return tokenToReturn.ToString();
        }

        public class AutenticacionServiceOptions
        {
            public const string AutenticacionService = "AutenticacionService";

            public string Issuer { get; set; }
            public string Audience { get; set; }
            public string SecretForKey { get; set; }
        }
    }
}
