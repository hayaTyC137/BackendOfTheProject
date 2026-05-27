using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controllers.Community
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

        [HttpGet("my")]
        [Authorize]
        public IActionResult GetMy() => Ok(_reportActions.GetMine(GetCurrentUserId()));

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] CreateReportRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "Укажите причину жалобы" });

            var (result, report) = _reportActions.Create(GetCurrentUserId(), request);

            if (result == CreateReportResult.ReporterUserNotFound)
                return Unauthorized(new { message = "Пользователь не найден" });

            if (result == CreateReportResult.ReportedUserNotFound)
                return NotFound(new { message = "Пользователь не найден" });

            if (result == CreateReportResult.SelfReport)
                return BadRequest(new { message = "Нельзя отправить жалобу на самого себя" });

            if (result == CreateReportResult.DuplicateOpenReport)
                return BadRequest(new { message = "У вас уже есть активная жалоба на этого пользователя" });

            return StatusCode(201, report);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult SetStatus(int id, [FromBody] UpdateReportStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest(new { message = "Укажите новый статус жалобы" });

            var (result, report) = _reportActions.SetStatus(
                id,
                request.Status,
                GetCurrentUserId(),
                GetCurrentUsername(),
                request.ModeratorComment);

            if (result == UpdateReportStatusResult.NotFound)
                return NotFound(new { message = $"Report {id} not found" });

            if (result == UpdateReportStatusResult.InvalidStatus)
                return BadRequest(new { message = "Недопустимый статус жалобы" });

            if (result == UpdateReportStatusResult.AlreadyClosed)
                return BadRequest(new { message = "Жалоба уже закрыта" });

            return Ok(report);
        }

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetCurrentUsername()
            => User.FindFirstValue(ClaimTypes.Name) ?? "moderator";
    }
}
