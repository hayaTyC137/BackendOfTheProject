using EgorkaCoins.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        // GET api/games — все игры
        [HttpGet]
        public IActionResult GetAll()
        {
            using var db = new AppDbContext();
            var games = db.Games.ToList();
            return Ok(games);
        }

        // GET api/games/valorant — одна игра
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            using var db = new AppDbContext();
            var game = db.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
                return NotFound(new { message = $"Game {id} not found" });

            return Ok(game);
        }

        // GET api/games/valorant/packages — пакеты конкретной игры
        [HttpGet("{id}/packages")]
        public IActionResult GetPackages(string id)
        {
            using var db = new AppDbContext();
            var packages = db.Packages
                .Where(p => p.GameId == id)
                .ToList();

            return Ok(packages);
        }
    }
}