using EgorkaCoins.Api.Contracts.Users;
using AutoMapper;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controllers.Users
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserActions _userActions;
        private readonly IWebHostEnvironment _environment;

        public UsersController(IMapper mapper, IWebHostEnvironment environment)
        {
            _userActions = new UserActions(mapper);
            _environment = environment;
        }

        [HttpGet("me")]
        public IActionResult GetMe()
            => Ok(_userActions.GetById(GetCurrentUserId()));

        [HttpPut("me")]
        public IActionResult UpdateMe([FromBody] UpdateUserRequest request)
        {
            var (result, user) = _userActions.Update(GetCurrentUserId(), request);

            if (result == UpdateUserResult.NotFound)
                return NotFound(new { message = "Пользователь не найден" });

            if (result == UpdateUserResult.UsernameTaken)
                return BadRequest(new { message = "Никнейм уже занят" });

            if (result == UpdateUserResult.InvalidUsername)
                return BadRequest(new { message = "Никнейм должен быть длиной от 3 до 24 символов" });

            return Ok(user);
        }

        [HttpPut("me/password")]
        public IActionResult ChangeMyPassword([FromBody] ChangePasswordRequest request)
        {
            var result = _userActions.ChangePassword(GetCurrentUserId(), request);

            if (result == ChangePasswordResult.NotFound)
                return NotFound(new { message = "Пользователь не найден" });

            if (result == ChangePasswordResult.InvalidCurrentPassword)
                return BadRequest(new { message = "Текущий пароль введён неверно" });

            if (result == ChangePasswordResult.InvalidNewPassword)
                return BadRequest(new { message = "Новый пароль должен содержать минимум 8 символов" });

            if (result == ChangePasswordResult.SamePassword)
                return BadRequest(new { message = "Новый пароль должен отличаться от текущего" });

            return Ok(new { message = "Пароль обновлён" });
        }

        [HttpPost("me/avatar")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateMyAvatar([FromForm] UploadAvatarRequest request)
        {
            var avatar = request.Avatar;

            if (avatar == null || avatar.Length == 0)
                return BadRequest(new { message = "Файл аватара не передан" });

            var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp" };

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Разрешены только PNG, JPG, JPEG и WEBP" });

            if (avatar.Length > 2 * 1024 * 1024)
                return BadRequest(new { message = "Размер файла не должен превышать 2 МБ" });

            var webRootPath = _environment.WebRootPath ??
                Path.Combine(_environment.ContentRootPath, "wwwroot");

            var avatarDirectory = Path.Combine(webRootPath, "uploads", "avatars");
            Directory.CreateDirectory(avatarDirectory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(avatarDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                avatar.CopyTo(stream);
            }

            var avatarUrl = $"/uploads/avatars/{fileName}";
            var currentUser = _userActions.GetById(GetCurrentUserId());
            DeleteOldAvatarIfNeeded(currentUser?.AvatarUrl, webRootPath);

            var (result, user) = _userActions.UpdateAvatar(GetCurrentUserId(), avatarUrl);
            if (result == UpdateAvatarResult.NotFound)
                return NotFound(new { message = "Пользователь не найден" });

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

        private static void DeleteOldAvatarIfNeeded(string? avatarUrl, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl) ||
                !avatarUrl.StartsWith("/uploads/avatars/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var relativePath = avatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var oldFilePath = Path.Combine(webRootPath, relativePath);

            if (global::System.IO.File.Exists(oldFilePath))
                global::System.IO.File.Delete(oldFilePath);
        }
    }
}
