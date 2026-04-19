using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserActions _userActions = new UserActions();

        // POST api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Заполните все поля" });
            }

            var user = _userActions.Register(request);

            if (user == null)
            {
                return BadRequest(new { message = "Пользователь с таким email или ником уже существует" });
            }

            // Сохраняем сессию в cookie
            HttpContext.Session.SetInt32("userId", user.Id);

            return StatusCode(201, user);
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Identifier) ||
                string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Заполните все поля" });
            }

            var user = _userActions.Login(request);

            if (user == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            // Сохраняем сессию в cookie
            HttpContext.Session.SetInt32("userId", user.Id);

            return Ok(user);
        }

        // POST api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Выход выполнен" });
        }

        // GET api/auth/me
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (userId == null)
            {
                return Unauthorized(new { message = "Не авторизован" });
            }

            var user = _userActions.GetById(userId.Value);

            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден" });
            }

            return Ok(user);
        }
    }
}