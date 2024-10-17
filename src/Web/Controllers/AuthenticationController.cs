using Application.Interfaces;
using Application.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ICustomAuthenticationService _customAuthenticationService;
        public AuthenticationController(IConfiguration config, ICustomAuthenticationService autenticacionService)
        {
            _config = config; 
            _customAuthenticationService = autenticacionService;
        }

        [HttpPost("authenticate")] 
        public async Task<ActionResult<string>> Autenticar(AuthenticationRequest authenticationRequest) 
        {
            string token = await _customAuthenticationService.AutenticarAsync(authenticationRequest);

            return Ok(token);
        }
    }
}
