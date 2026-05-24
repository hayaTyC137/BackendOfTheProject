using AutoMapper;
using EgorkaCoins.Api.Services;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserActions _userActions;
        private readonly TokenService _tokenService;

        public AuthController(IMapper mapper, TokenService tokenService)
        {
            _userActions = new UserActions(mapper);
            _tokenService = tokenService;
        }

        // Регистрация
        [HttpPost("register")]
        [AllowAnonymous]
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
                return BadRequest(new { message = "Пользователь с таким email или ником уже существует" });

            var token = _tokenService.GenerateToken(user.Id, user.Username, user.Role);
            return StatusCode(201, new { token, user });
        }

        // Вход
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Identifier) ||
                string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Заполните все поля" });
            }

            var user = _userActions.Login(request);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var token = _tokenService.GenerateToken(user.Id, user.Username, user.Role);
            return Ok(new { token, user });
        }

        // Выход
        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            // JWT без сессии
            return Ok(new { message = "Выход выполнен" });
        }

        // Текущий пользователь
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _userActions.GetById(userId);

            if (user == null)
                return Unauthorized(new { message = "Пользователь не найден" });

            return Ok(user);
        }
    }
}
