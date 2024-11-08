using Application.Interfaces;
using Application.Models.Order;
using Application.Shared.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filters = null, [FromQuery] Sorter? sorter = null, [FromQuery] Paginator? paginator = null)
        {
            try
            {
                var options = new Options(filters, sorter, paginator);
                var orders = await _orderService.GetAll();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("history")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetAllByUser([FromQuery] Sorter? sorter = null, [FromQuery] Paginator? paginator = null)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token.");
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized("Invalid user ID format in token.");
                }

                var options = new Options($"IdUser:{userIdClaim.Value}:1", sorter, paginator);

                var orders = await _orderService.GetAll(options);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token.");
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized("Invalid user ID format in token.");
                }

                // Set the user ID in the DTO (or create a new DTO without IdUser)
                orderDto.IdUser = userId;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.Create(orderDto);

                return Ok(order);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
