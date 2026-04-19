using EgorkaCoins.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        // GET api/packages — все пакеты
        [HttpGet]
        public IActionResult GetAll()
        {
            using var db = new AppDbContext();
            var packages = db.Packages.ToList();
            return Ok(packages);
        }

        // GET api/packages/vp1 — один пакет
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            using var db = new AppDbContext();
            var package = db.Packages.FirstOrDefault(p => p.Id == id);

            if (package == null)
                return NotFound(new { message = $"Package {id} not found" });

            return Ok(package);
        }
    }
}