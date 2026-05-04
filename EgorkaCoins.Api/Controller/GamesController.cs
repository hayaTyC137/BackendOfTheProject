using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameActions _gameActions = new GameActions();

        // GET api/games
        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _gameActions.GetAll();
            return Ok(games);
        }

        // GET api/games/valorant
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var game = _gameActions.GetById(id);
            if (game == null)
                return NotFound(new { message = $"Game {id} not found" });
            return Ok(game);
        }

        // GET api/games/valorant/packages
        [HttpGet("{id}/packages")]
        public IActionResult GetPackages(string id)
        {
            var packages = _gameActions.GetPackages(id);
            return Ok(packages);
        }

        // POST api/games
        [HttpPost]
        public IActionResult Create([FromBody] Game game)
        {
            var created = _gameActions.Create(game);
            if (created == null)
                return BadRequest(new { message = "Game with this ID already exists" });
            return StatusCode(201, created);
        }

        // PUT api/games/valorant
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Game updated)
        {
            var game = _gameActions.Update(id, updated);
            if (game == null)
                return NotFound(new { message = $"Game {id} not found" });
            return Ok(game);
        }

        // DELETE api/games/valorant
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _gameActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Game {id} not found" });
            return NoContent();
        }
    }
}