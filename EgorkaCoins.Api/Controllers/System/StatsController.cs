using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controllers.System
{
    [Route("api/admin")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        [HttpGet("stats")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetStats()
        {
            using var db = new AppDbContext();
            var today = DateTime.UtcNow.Date;

            var stats = new AdminStatsDto
            {
                TotalUsers = db.Users.Count(),
                OrdersToday = db.Orders.Count(o => o.CreatedAt >= today),
                TotalRevenue = db.Orders.Sum(o => (decimal?)o.Price) ?? 0,
                OpenReports = db.Reports.Count(r => r.Status == "open")
            };

            return Ok(stats);
        }
    }
}
