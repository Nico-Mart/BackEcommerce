using Application.Interfaces;
using Application.Models.User;
using Application.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ReadUserDto>>> GetAll(Options? options = null)
        {
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

        [HttpPost("create-and-verify")]
        public async Task<ActionResult> CreateAndVerify(CreateUserDto userDto)
        {
            var createdUser = await _userService.Create(userDto);

            var token = await _userService.GenerateVerificationToken(createdUser.Id);

            return Ok(new { Token = token });
        }
        [HttpPost("create-range-and-verify")]
        public async Task<ActionResult> CreateRangeAndVerify(ICollection<CreateUserDto> userDtos)
        {
            var createdUsers = await _userService.CreateRange(userDtos);

            var tokens = new List<string>();
            foreach (var user in createdUsers)
            {
                var token = await _userService.GenerateVerificationToken(user.Id);
                tokens.Add(token);
            }

            return Ok(new { Tokens = tokens });
        }
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
