using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameActions _gameActions = new GameActions();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll() => Ok(_gameActions.GetAll());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(string id)
        {
            var game = _gameActions.GetById(id);
            if (game == null)
                return NotFound(new { message = $"Game {id} not found" });
            return Ok(game);
        }

        [HttpGet("{id}/packages")]
        [AllowAnonymous]
        public IActionResult GetPackages(string id) => Ok(_gameActions.GetPackages(id));

        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Create([FromBody] Game game)
        {
            var created = _gameActions.Create(game);
            if (created == null)
                return BadRequest(new { message = "Game with this ID already exists" });
            return StatusCode(201, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Update(string id, [FromBody] Game updated)
        {
            var game = _gameActions.Update(id, updated);
            if (game == null)
                return NotFound(new { message = $"Game {id} not found" });
            return Ok(game);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Delete(string id)
        {
            var result = _gameActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Game {id} not found" });
            return NoContent();
        }
    }
}