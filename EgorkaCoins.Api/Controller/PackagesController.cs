using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly PackageActions _packageActions = new PackageActions();

        // GET api/packages
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _packageActions.GetAll();
            return Ok(packages);
        }

        // GET api/packages/vp1
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var package = _packageActions.GetById(id);
            if (package == null)
                return NotFound(new { message = $"Package {id} not found" });
            return Ok(package);
        }

        // POST api/packages
        [HttpPost]
        public IActionResult Create([FromBody] Package package)
        {
            var created = _packageActions.Create(package);
            if (created == null)
                return BadRequest(new { message = "Package with this ID already exists" });
            return StatusCode(201, created);
        }

        // PUT api/packages/vp1
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Package updated)
        {
            var package = _packageActions.Update(id, updated);
            if (package == null)
                return NotFound(new { message = $"Package {id} not found" });
            return Ok(package);
        }

        // DELETE api/packages/vp1
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _packageActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Package {id} not found" });
            return NoContent();
        }
    }
}