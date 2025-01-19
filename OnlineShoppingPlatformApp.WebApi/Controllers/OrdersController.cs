using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatformApp.Business.Operations.Order;
using OnlineShoppingPlatformApp.Business.Operations.Order.Dtos;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.WebApi.Filters;
using OnlineShoppingPlatformApp.WebApi.Jwt;
using OnlineShoppingPlatformApp.WebApi.Models.Order;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatformApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private int UserId => int.Parse(HttpContext.User.Claims.First(c => c.Type == JwtCustomClaimNames.Id).Value);


        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _orderService.GetUserOrders(UserId);
            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderById(id);

            if (result == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Kayıt Bulunamadı",
                    DetailedMessage = $"{id} numaralı sipariş bulunamadı."
                });
            }
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var createOrderDto = new CreateOrderDto
            {
                CartItems = request.CartItems.Select(x => new CartItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList(),
                UserId = UserId
            };

            var result = await _orderService.CreateOrder(createOrderDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrders();
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var updateOrderStatusDto = new UpdateOrderStatusDto
            {
                Status = request.Status
            };
            var result = await _orderService.UpdateOrderStatus(id, updateOrderStatusDto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
        {
            var updateOrderDto = new UpdateOrderDto
            {
                OrderDate = request.OrderDate,
                Status = request.Status,
                CartItems = request.CartItems.Select(x => new CartItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };

            var result = await _orderService.UpdateOrder(id, updateOrderDto);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var result = await _orderService.GetUserOrders(userId);
            return Ok(result);
        }

    }
}