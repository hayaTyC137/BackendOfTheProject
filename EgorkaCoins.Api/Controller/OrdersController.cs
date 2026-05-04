using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderActions _orderActions = new OrderActions();

        // GET api/orders — заказы текущего пользователя
        [HttpGet]
        public IActionResult GetMyOrders()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var orders = _orderActions.GetByUser(userId.Value);
            return Ok(orders);
        }

        // GET api/orders/all — все заказы (для admin/moderator)
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var orders = _orderActions.GetAll();
            return Ok(orders);
        }

        // GET api/orders/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var order = _orderActions.GetById(id);
            if (order == null)
                return NotFound(new { message = $"Order {id} not found" });

            return Ok(order);
        }

        // POST api/orders — создать заказы из корзины
        [HttpPost]
        public IActionResult Create([FromBody] List<CreateOrderRequest> items)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            if (items == null || items.Count == 0)
                return BadRequest(new { message = "Корзина пуста" });

            var orders = _orderActions.Create(userId.Value, items);
            return StatusCode(201, orders);
        }

        // PUT api/orders/5/status — обновить статус
        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var order = _orderActions.UpdateStatus(id, request.Status);
            if (order == null)
                return NotFound(new { message = $"Order {id} not found" });

            return Ok(order);
        }

        // DELETE api/orders/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var result = _orderActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Order {id} not found" });

            return NoContent();
        }
    }
}