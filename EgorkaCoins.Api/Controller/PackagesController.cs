using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route(template: "api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        // In-memory хранилище — точно так же как у преподавателя с User
        // static означает что список живёт пока работает сервер, общий для всех запросов
        private static List<Package> _packages = new();

        // GET api/packages — вернуть все пакеты
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_packages);
        }

        // GET api/packages/vp1 — вернуть один пакет по его строковому Id
        // Обрати внимание: здесь string id, а не int — потому что у тебя "vp1", "gem2"
        [HttpGet(template: "{id}")]
        public IActionResult GetById(string id)
        {
            var package = _packages.FirstOrDefault(p => p.Id == id);

            if (package == null)
            {
                return NotFound(new { Message = $"Package with ID {id} not found" });
            }

            return Ok(package);
        }

        // GET api/packages/game/valorant — все пакеты конкретной игры
        // Это дополнительный эндпоинт которого нет в примере преподавателя,
        // но он нужен твоему фронту — ProductPage запрашивает пакеты по gameId
        [HttpGet(template: "game/{gameId}")]
        public IActionResult GetByGame(string gameId)
        {
            var packages = _packages.Where(p => p.GameId == gameId).ToList();
            return Ok(packages);
        }

        // POST api/packages — создать новый пакет
        [HttpPost]
        public IActionResult Create([FromBody] Package package)
        {
            if (string.IsNullOrEmpty(package.Id) || string.IsNullOrEmpty(package.GameId))
            {
                return BadRequest(new { Message = "Id and GameId are required" });
            }

            // Проверяем что пакет с таким Id ещё не существует
            if (_packages.Any(p => p.Id == package.Id))
            {
                return BadRequest(new { Message = $"Package with ID {package.Id} already exists" });
            }

            _packages.Add(package);
            // Created возвращает 201 и заголовок Location с адресом нового ресурса
            return Created(uri: $"/api/packages/{package.Id}", value: package);
        }

        // PUT api/packages/vp1 — обновить пакет
        [HttpPut(template: "{id}")]
        public IActionResult Update(string id, [FromBody] Package updatedPackage)
        {
            var existing = _packages.FirstOrDefault(p => p.Id == id);

            if (existing == null)
            {
                return NotFound(new { Message = $"Package with ID {id} not found" });
            }

            // Обновляем только те поля которые имеет смысл менять
            existing.Label = updatedPackage.Label;
            existing.Amount = updatedPackage.Amount;
            existing.Price = updatedPackage.Price;
            existing.OldPrice = updatedPackage.OldPrice;
            existing.Bonus = updatedPackage.Bonus;
            existing.Badge = updatedPackage.Badge;
            existing.Popular = updatedPackage.Popular;
            // Id и GameId не меняем — это идентификаторы

            return Ok(existing);
        }

        // DELETE api/packages/vp1 — удалить пакет
        [HttpDelete(template: "{id}")]
        public IActionResult Delete(string id)
        {
            var package = _packages.FirstOrDefault(p => p.Id == id);

            if (package == null)
            {
                return NotFound(new { Message = $"Package with ID {id} not found" });
            }

            _packages.Remove(package);
            return NoContent(); // 204 — удалено успешно, тело ответа пустое
        }

    }
}
