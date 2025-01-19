using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatformApp.Business.Operations.Product;
using OnlineShoppingPlatformApp.Business.Operations.Product.Dtos;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.WebApi.Filters;
using OnlineShoppingPlatformApp.WebApi.Models.Product;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllProducts();
            return Ok(result);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetProductById(id);
            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> Create([FromBody] AddProductRequest request)
        {
            var addProductDto = new AddProductDto
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
            };
            var result = await _productService.AddProduct(addProductDto);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var updateProductDto = new UpdateProductDto
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock
            };
            var result = await _productService.UpdateProduct(id, updateProductDto);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}