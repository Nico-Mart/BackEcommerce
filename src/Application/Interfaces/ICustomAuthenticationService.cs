using Application.Models.Request;

namespace Application.Interfaces
{
    public interface ICustomAuthenticationService
    {
        Task<string> AutenticarAsync(AuthenticationRequest authenticationRequest);
    }
}
