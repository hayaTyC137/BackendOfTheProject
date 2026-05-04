using AutoMapper;
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

        // GET api/users/me — профиль текущего пользователя
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var user = _userActions.GetById(userId.Value);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            return Ok(user);
        }

        // PUT api/users/me — обновить профиль
        [HttpPut("me")]
        public IActionResult UpdateMe([FromBody] UpdateUserRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var user = _userActions.Update(userId.Value, request);
            if (user == null)
                return BadRequest(new { message = "Имя или email уже заняты" });

            return Ok(user);
        }

        // DELETE api/users/me — удалить свой аккаунт
        [HttpDelete("me")]
        public IActionResult DeleteMe()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var result = _userActions.Delete(userId.Value);
            if (!result)
                return NotFound(new { message = "Пользователь не найден" });

            HttpContext.Session.Clear();
            return NoContent();
        }

        // GET api/users — все пользователи (admin)
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var users = _userActions.GetAll();
            return Ok(users);
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var user = _userActions.GetById(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });

            return Ok(user);
        }

        // PUT api/users/5/ban — забанить / разбанить
        [HttpPut("{id}/ban")]
        public IActionResult SetBan(int id, [FromBody] SetBanRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var user = _userActions.SetBan(id, request.IsBanned);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });

            return Ok(user);
        }

        // PUT api/users/5/role — сменить роль
        [HttpPut("{id}/role")]
        public IActionResult SetRole(int id, [FromBody] SetRoleRequest request)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var user = _userActions.SetRole(id, request.Role);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });

            return Ok(user);
        }

        // DELETE api/users/5 — удалить пользователя (admin)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var result = _userActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"User {id} not found" });

            return NoContent();
        }
    }
}