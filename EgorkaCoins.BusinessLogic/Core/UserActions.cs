using EgorkaCoins.Domain;
using EgorkaCoins.Helpers;
using EgorkaCoins.Helpers.DTOs;
using EgorkaCoins.DataAccess.Context;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class UserActions
    {
        // Регистрация
        public UserDto? Register(RegisterRequest request)
        {
            using var db = new AppDbContext();

            // Проверяем что email и username не заняты
            var exists = db.Users.Any(u =>
                u.Email == request.Email.ToLower() ||
                u.Username == request.Username.ToLower());

            if (exists) return null;

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                Password = PasswordHelper.Hash(request.Password),
                Role = "user",
                Balance = 0,
                TotalSpent = 0,
                OrdersCount = 0,
                Level = 1,
                Xp = 0,
                XpToNext = 1000,
                Verified = false,
                IsBanned = false,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);
            db.SaveChanges();

            return ToDto(user);
        }

        // Логин
        public UserDto? Login(LoginRequest request)
        {
            using var db = new AppDbContext();

            var identifier = request.Identifier.Trim().ToLower();

            var user = db.Users.FirstOrDefault(u =>
                u.Email == identifier ||
                u.Username.ToLower() == identifier);

            if (user == null) return null;
            if (user.IsBanned) return null;
            if (!PasswordHelper.Verify(request.Password, user.Password)) return null;

            return ToDto(user);
        }

        // Получить пользователя по Id
        public UserDto? GetById(int id)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            return user == null ? null : ToDto(user);
        }

        // Конвертация User → UserDto
        private static UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Balance = user.Balance,
            TotalSpent = user.TotalSpent,
            OrdersCount = user.OrdersCount,
            Level = user.Level,
            Xp = user.Xp,
            XpToNext = user.XpToNext,
            Verified = user.Verified,
            IsBanned = user.IsBanned,
            CreatedAt = user.CreatedAt
        };
    }
}