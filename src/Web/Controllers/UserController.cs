using Application.Interfaces;
using Application.Models.User;
using Application.Shared.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ReadUserDto>>> GetAll([FromQuery] string? filters = null, [FromQuery] Sorter? sorter = null, [FromQuery] Paginator? paginator = null)
        {
            var options = new Options(filters, sorter, paginator);
            var users = await _userService.GetAll(options);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadUserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        //[Authorize(Roles = "sysadmin")]
        [HttpPost("create-and-verify")]
        public async Task<ActionResult> CreateAndVerify(CreateUserDto userDto)
        {
            try
            {
                var createdUser = await _userService.Create(userDto);
                return Ok("Se ha enviado un correo de verificación. Por favor revisa tu bandeja de entrada.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify")]
        public async Task<ActionResult> ActivateUser([FromQuery] string token)
        {
            try
            {
                await _userService.ActivateAccount(token);
                return Ok("Cuenta verificada y creada exitosamente.");
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest($"Falló la verificación: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }

        //[HttpPost("create-range-and-verify")]
        //public async Task<ActionResult> CreateRangeAndVerify(ICollection<CreateUserDto> userDtos)
        //{
        //    var createdUsers = await _userService.CreateRange(userDtos);

        //    var tokens = new List<string>();
        //    foreach (var user in createdUsers)
        //    {
        //        var token = await _userService.GenerateVerificationToken(user.Email);
        //        tokens.Add(token);
        //    }

        //    return Ok(new { Tokens = tokens });
        //}
        [HttpPut]
        public async Task<ActionResult> Update(UpdateUserDto userDto)
        {
            await _userService.Update(userDto);
            return NoContent();
        }
        [HttpPut("update-range")]
        public async Task<ActionResult<int>> UpdateRange(ICollection<UpdateUserDto> userDtos)
        {
            var updatedCount = await _userService.UpdateRange(userDtos);
            return Ok(updatedCount);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteRange(List<int> ids)
        {
            await _userService.DeleteRange(ids);
            return NoContent();
        }
    }
}
