using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
