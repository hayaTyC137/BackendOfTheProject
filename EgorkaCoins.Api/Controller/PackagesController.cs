using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly PackageActions _packageActions = new PackageActions();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll() => Ok(_packageActions.GetAll());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(string id)
        {
            var package = _packageActions.GetById(id);
            if (package == null)
                return NotFound(new { message = $"Package {id} not found" });
            return Ok(package);
        }

        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Create([FromBody] Package package)
        {
            var created = _packageActions.Create(package);
            if (created == null)
                return BadRequest(new { message = "Package with this ID already exists" });
            return StatusCode(201, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Update(string id, [FromBody] Package updated)
        {
            var package = _packageActions.Update(id, updated);
            if (package == null)
                return NotFound(new { message = $"Package {id} not found" });
            return Ok(package);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Delete(string id)
        {
            var result = _packageActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Package {id} not found" });
            return NoContent();
        }
    }
}