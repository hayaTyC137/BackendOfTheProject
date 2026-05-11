using AutoMapper;
using EgorkaCoins.Api.Filters;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserActions _userActions;

        public UsersController(IMapper mapper)
        {
            _userActions = new UserActions(mapper);
        }

        // GET api/users/me — любой залогиненный
        [HttpGet("me")]
        [RequireAuth]
        public IActionResult GetMe()
        {
            var userId = HttpContext.Session.GetInt32("userId")!.Value;
            var user = _userActions.GetById(userId);
            return Ok(user);
        }

        // PUT api/users/me — любой залогиненный
        [HttpPut("me")]
        [RequireAuth]
        public IActionResult UpdateMe([FromBody] UpdateUserRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId")!.Value;
            var user = _userActions.Update(userId, request);
            if (user == null)
                return BadRequest(new { message = "Имя или email уже заняты" });
            return Ok(user);
        }

        // DELETE api/users/me — любой залогиненный
        [HttpDelete("me")]
        [RequireAuth]
        public IActionResult DeleteMe()
        {
            var userId = HttpContext.Session.GetInt32("userId")!.Value;
            var result = _userActions.Delete(userId);
            if (!result)
                return NotFound(new { message = "Пользователь не найден" });

            HttpContext.Session.Clear();
            return NoContent();
        }

        // GET api/users — только admin/moderator
        [HttpGet]
        [RequireAuth]
        [AdminMod]
        public IActionResult GetAll()
        {
            var users = _userActions.GetAll();
            return Ok(users);
        }

        // GET api/users/5 — только admin/moderator
        [HttpGet("{id}")]
        [RequireAuth]
        [AdminMod]
        public IActionResult GetById(int id)
        {
            var user = _userActions.GetById(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        // PUT api/users/5/ban — только admin/moderator
        [HttpPut("{id}/ban")]
        [RequireAuth]
        [AdminMod]
        public IActionResult SetBan(int id, [FromBody] SetBanRequest request)
        {
            var user = _userActions.SetBan(id, request.IsBanned);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        // PUT api/users/5/role — только admin/moderator
        [HttpPut("{id}/role")]
        [RequireAuth]
        [AdminMod]
        public IActionResult SetRole(int id, [FromBody] SetRoleRequest request)
        {
            var user = _userActions.SetRole(id, request.Role);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        // DELETE api/users/5 — только admin/moderator
        [HttpDelete("{id}")]
        [RequireAuth]
        [AdminMod]
        public IActionResult Delete(int id)
        {
            var result = _userActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"User {id} not found" });
            return NoContent();
        }
    }
}