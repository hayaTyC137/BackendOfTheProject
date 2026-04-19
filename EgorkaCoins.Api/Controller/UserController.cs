using EgorkaCoins.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static List<User> _users = new();
        private static int _nextId = 1;

        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            return Ok(_users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id) 
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if(user == null)
            {
                return NotFound(new {Message = $"User with id {id} not found."});
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            user.Id = _nextId++;
            user.CreatedAt = DateTime.UtcNow;

            _users.Add(user);

            return Created($"/api/user/{user.Id}", user);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound(new { Message = $"User with id {id} not found." });
            }
            existingUser.Username = updatedUser.Username;
            existingUser.Password = updatedUser.Password;
            existingUser.Role = updatedUser.Role;
            existingUser.Balance = updatedUser.Balance;
            existingUser.Email = updatedUser.Email;
            return Ok(existingUser);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with id {id} not found." });
            }
            _users.Remove(user);
            return NoContent();
        }
    }
}
