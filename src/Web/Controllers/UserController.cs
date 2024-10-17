using Application.Interfaces;
using Application.Models.Product;
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
        public async Task<ActionResult<ICollection<ReadUserDto>>> GetAll([FromQuery] Options? options)
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

        [HttpPost]
        public async Task<ActionResult<ReadUserDto>> Create([FromBody] CreateUserDto userDto)
        {
            var createdUser = await _userService.Create(userDto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto userDto)
        {
            await _userService.Update(userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteRange([FromBody] List<int> ids)
        {
            await _userService.DeleteRange(ids);
            return NoContent();
        }

        [HttpPut("update-range")]
        public async Task<ActionResult<int>> UpdateRange([FromBody] ICollection<UpdateUserDto> userDtos)
        {
            var updatedCount = await _userService.UpdateRange(userDtos);
            return Ok(updatedCount);
        }

    }
}
