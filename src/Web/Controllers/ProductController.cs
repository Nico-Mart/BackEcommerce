using Application.Interfaces;
using Application.Models.Product;
using Application.Models.ProductVariant;
using Application.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;

        public ProductController(IProductService productService, IProductVariantService productVariantService)
        {
            _productService = productService;
            _productVariantService = productVariantService;
        }

        #region Product endpoints
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filters = null, [FromQuery] Sorter? sorter = null, [FromQuery] Paginator? paginator = null)
        {
            try
            {
                var options = new Options(filters, sorter, paginator);
                var products = await _productService.GetAll(options);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.Create(productDto);

                return Ok(product);
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

        [HttpPost("range")]
        public async Task<IActionResult> CreateRange([FromBody] List<CreateProductDto> productDtos)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var products = await _productService.CreateRange(productDtos);

                return Ok(products);
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _productService.Update(productDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("range")]
        public async Task<IActionResult> UpdateRange([FromBody] List<UpdateProductDto> productDtos)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var rows = await _productService.UpdateRange(productDtos);
                return Ok(rows);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteRange(List<int> ids)
        {
            try
            {
                var rows = await _productService.DeleteRange(ids);
                return Ok(rows);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region ProductVariant endpoints
        [HttpGet("{id}/variants")]
        public async Task<IActionResult> GetAllVariantsByProductId(int id)
        {
            try
            {
                var productVariants = await _productVariantService.GetAll(id);
                return Ok(productVariants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("variant")]
        public async Task<IActionResult> GetAllVariants([FromQuery] string? filters = null, [FromQuery] Sorter? sorter = null, [FromQuery] Paginator? paginator = null)
        {
            try
            {
                var options = new Options(filters, sorter, paginator);
                var productVariants = await _productVariantService.GetAll(options);
                return Ok(productVariants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("variant")]
        public async Task<IActionResult> VariantUpdate([FromBody] UpdateProductVariantDto productVariantDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _productVariantService.Update(productVariantDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("variant/range")]
        public async Task<IActionResult> VariantUpdateRange([FromBody] List<UpdateProductVariantDto> productVariants)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var rows = await _productVariantService.UpdateRange(productVariants);
                return Ok(rows);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("variant/{id}")]
        public async Task<IActionResult> VariantDelete(int id)
        {
            try
            {
                await _productVariantService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("variant/range")]
        public async Task<IActionResult> VariantDeleteRange(List<int> ids)
        {
            try
            {
                var rows = await _productVariantService.DeleteRange(ids);
                return Ok(rows);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion
    }
}
