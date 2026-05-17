using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EgorkaCoins.Api.Filters;
using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Helpers.DTOs;


namespace EgorkaCoins.Api.Controller
{
    [Route("api/admin")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        // GET api/admin/stats
        [HttpGet("stats")]
        [RequireAuth]
        [AdminMod]
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