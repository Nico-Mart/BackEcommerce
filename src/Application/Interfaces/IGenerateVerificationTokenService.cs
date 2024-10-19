using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGenerateVerificationTokenService
    {
        string GenerateVerificationToken(User user);
        Task SendVerificationEmail(string email, string token);
    }
}
