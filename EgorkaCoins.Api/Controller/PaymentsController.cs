using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentActions _paymentActions = new PaymentActions();

        [HttpGet]
        public IActionResult GetMyPayments()
        {
            return Ok(_paymentActions.GetByUser(GetCurrentUserId()));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var result = _paymentActions.CreatePaid(GetCurrentUserId(), request);
                return StatusCode(201, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
