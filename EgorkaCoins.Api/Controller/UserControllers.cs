using AutoMapper;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserActions _userActions;

        public UsersController(IMapper mapper)
        {
            _userActions = new UserActions(mapper);
        }

        [HttpGet("me")]
        public IActionResult GetMe()
            => Ok(_userActions.GetById(GetCurrentUserId()));

        [HttpPut("me")]
        public IActionResult UpdateMe([FromBody] UpdateUserRequest request)
        {
            var user = _userActions.Update(GetCurrentUserId(), request);
            if (user == null)
                return BadRequest(new { message = "Имя или email уже заняты" });
            return Ok(user);
        }

        [HttpDelete("me")]
        public IActionResult DeleteMe()
        {
            var result = _userActions.Delete(GetCurrentUserId());
            if (!result)
                return NotFound(new { message = "Пользователь не найден" });
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetAll() => Ok(_userActions.GetAll());

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult GetById(int id)
        {
            var user = _userActions.GetById(id);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        [HttpPut("{id}/ban")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult SetBan(int id, [FromBody] SetBanRequest request)
        {
            var user = _userActions.SetBan(id, request.IsBanned);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult SetRole(int id, [FromBody] SetRoleRequest request)
        {
            var user = _userActions.SetRole(id, request.Role);
            if (user == null)
                return NotFound(new { message = $"User {id} not found" });
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public IActionResult Delete(int id)
        {
            var result = _userActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"User {id} not found" });
            return NoContent();
        }

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}