using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Application.Services
{
    public class GenerateVerificationTokenService : IGenerateVerificationTokenService
    {
        private readonly IEmailService _emailService;
        private readonly string _baseUrl; 

        public GenerateVerificationTokenService(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _baseUrl = configuration["Verification:BaseUrl"]; 
        }
        public string GenerateVerificationToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("thisisthesecretforgeneratingakey(mustbeatleast32bitlong)"); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("IsActive", user.IsActive.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task SendVerificationEmail(string email, string token)
        {
            var verificationLink = $"{_baseUrl}/verify?token={token}";
            var subject = "Verify your account";
            var body = $"<p>Please verify your account by clicking on the link below:</p><a href='{verificationLink}'>Verify Account</a>";

            await _emailService.SendEmailAsync(email, subject, body);
        }

    }
}
