using Application.Interfaces;
using Application.Models.Password;
using Application.Models.Register;
using Application.Models.Request;
using Application.Models.User;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ICustomAuthenticationService _customAuthenticationService;

        public ClientController(IUserService userService, IMapper mapper, IConfiguration configuration, ICustomAuthenticationService customAuthenticationService)
        {
            _userService = userService;
            _mapper = mapper;
            _config = configuration;
            _customAuthenticationService = customAuthenticationService;

        }

        [HttpPost("RegisterClient")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(CreateRegisterUserDto registerDto)
        {
            try
            {
                var userDto = _mapper.Map<CreateUserDto>(registerDto);
                userDto.Role = Role.Client;

                var createdUser = await _userService.Create(userDto);
                return Ok("Se ha enviado un correo de verificación. Por favor revisa tu bandeja de entrada.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error durante el registro: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginClient(AuthenticationRequest authenticationRequest)
        {
            string token = await _customAuthenticationService.AutenticarAsync(authenticationRequest);

            return Ok(token);
        }

        [HttpPost("ResetPassword-Using-Email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordEmail([FromBody] EmailResetPassword emailReset)
        {
            try
            {
                await _userService.RequestPasswordReset(emailReset.Email);
                return Ok("Se ha enviado un correo de verificación. Por favor revisa tu bandeja de entrada.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error durante el restablecimiento de contraseña: {ex.Message}");
            }
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _userService.ResetPassword(resetPasswordDto);
                return Ok("Contraseña reseteada con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error durante el restablecimiento de contraseña: {ex.Message}");
            }
        }

        [HttpPut("UpdateClient/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody]UpdateRegisterUserDto userDto)
        {
            
            var user = _mapper.Map<UpdateUserDto>(userDto);
            user.Id = id;
            await _userService.Update(user);

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }

    }

}
