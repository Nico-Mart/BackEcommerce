using Application.Interfaces;
using Application.Models.Password;
using Application.Models.Register;
using Application.Models.User;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ClientController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;

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

        [HttpPatch("UpdateClient/{id}")]
        public async Task<ActionResult> UpdatePartial(int id, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }


            patchDoc.ApplyTo(user, error =>
            {
                if (error != null)
                {
                    ModelState.AddModelError(error.AffectedObject.ToString(), error.ErrorMessage);
                }
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.Update(user);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }

    }

}
