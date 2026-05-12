using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EgorkaCoins.Api.Filters;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportActions _reportActions = new ReportActions();

        // GET api/reports — все жалобы (admin/moderator)
        [HttpGet]
        [RequireAuth]
        [AdminMod]
        public IActionResult GetAll()
        {
            return Ok(_reportActions.GetAll());
        }

        // GET api/reports/open — только открытые
        [HttpGet("open")]
        [RequireAuth]
        [AdminMod]
        public IActionResult GetOpen()
        {
            return Ok(_reportActions.GetOpen());
        }

        // POST api/reports — создать жалобу (любой залогиненный)
        [HttpPost]
        [RequireAuth]
        public IActionResult Create([FromBody] CreateReportRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId")!.Value;

            if (string.IsNullOrEmpty(request.Reason))
                return BadRequest(new { message = "Укажите причину жалобы" });

            var report = _reportActions.Create(userId, request);
            if (report == null)
                return NotFound(new { message = "Пользователь не найден" });

            return StatusCode(201, report);
        }

        // PUT api/reports/5/resolve — закрыть жалобу
        [HttpPut("{id}/resolve")]
        [RequireAuth]
        [AdminMod]
        public IActionResult Resolve(int id)
        {
            var report = _reportActions.Resolve(id);
            if (report == null)
                return NotFound(new { message = $"Report {id} not found" });

            return Ok(report);
        }
    }
}