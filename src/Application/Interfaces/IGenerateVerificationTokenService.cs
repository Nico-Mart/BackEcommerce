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
        string GenerateVerificationToken(string email);

        string ValidateVerificationToken(string token);
    }
}
