using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportActions _reportActions = new ReportActions();

        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetAll() => Ok(_reportActions.GetAll());

        [HttpGet("open")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetOpen() => Ok(_reportActions.GetOpen());

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] CreateReportRequest request)
        {
            if (string.IsNullOrEmpty(request.Reason))
                return BadRequest(new { message = "Укажите причину жалобы" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var report = _reportActions.Create(userId, request);

            if (report == null)
                return NotFound(new { message = "Пользователь не найден" });

            return StatusCode(201, report);
        }

        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Resolve(int id)
        {
            var report = _reportActions.Resolve(id);
            if (report == null)
                return NotFound(new { message = $"Report {id} not found" });
            return Ok(report);
        }
    }
}