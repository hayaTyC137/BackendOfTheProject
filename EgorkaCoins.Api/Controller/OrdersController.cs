using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderActions _orderActions = new OrderActions();

        [HttpGet]
        public IActionResult GetMyOrders()
        {
            return Ok(_orderActions.GetByUser(GetCurrentUserId()));
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetAll() => Ok(_orderActions.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _orderActions.GetById(id);
            if (order == null)
                return NotFound(new { message = $"Order {id} not found" });
            return Ok(order);
        }

        [HttpPost]
        public IActionResult Create([FromBody] List<CreateOrderRequest> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest(new { message = "Корзина пуста" });

            var orders = _orderActions.Create(GetCurrentUserId(), items);
            return StatusCode(201, orders);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var order = _orderActions.UpdateStatus(id, request.Status);
            if (order == null)
                return NotFound(new { message = $"Order {id} not found" });
            return Ok(order);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Delete(int id)
        {
            var result = _orderActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Order {id} not found" });
            return NoContent();
        }

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}