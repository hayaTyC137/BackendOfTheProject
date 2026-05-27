using AutoMapper;
using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class UserActions
    {
        private readonly IMapper _mapper;

        public UserActions(IMapper mapper)
        {
            _mapper = mapper;
        }

        // Регистрация
        public UserDto? Register(RegisterRequest request)
        {
            using var db = new AppDbContext();

            var email = request.Email.Trim().ToLower();
            var username = request.Username.Trim();
            var usernameLower = username.ToLower();

            var exists = db.Users.Any(u =>
                u.Email == email ||
                u.Username.ToLower() == usernameLower);

            if (exists) return null;

            var user = new User
            {
                Username = username,
                Email = email,
                Password = PasswordHelper.Hash(request.Password),
                Role = "user",
                Balance = 0,
                TotalSpent = 0,
                OrdersCount = 0,
                AvatarUrl = string.Empty,
                Level = 1,
                Xp = 0,
                XpToNext = 1000,
                Verified = false,
                IsBanned = false,
                NotifyOrders = true,
                NotifyPromo = false,
                NotifySecurity = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);
            db.SaveChanges();

            return _mapper.Map<UserDto>(user);
        }

        // Вход
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

            return _mapper.Map<UserDto>(user);
        }

        // Пользователь по id
        public UserDto? GetById(int id)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        // Все пользователи
        public List<UserDto> GetAll()
        {
            using var db = new AppDbContext();
            return db.Users.Select(u => _mapper.Map<UserDto>(u)).ToList();
        }

        // Обновление профиля
        public (UpdateUserResult Result, UserDto? User) Update(int id, UpdateUserRequest request)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return (UpdateUserResult.NotFound, null);

            var username = request.Username.Trim();
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 24)
                return (UpdateUserResult.InvalidUsername, null);

            if (!string.Equals(user.Username, username, StringComparison.Ordinal))
            {
                var taken = db.Users.Any(u =>
                    u.Id != id &&
                    u.Username.ToLower() == username.ToLower());

                if (taken)
                    return (UpdateUserResult.UsernameTaken, null);
            }

            user.Username = username;
            user.NotifyOrders = request.NotifyOrders;
            user.NotifyPromo = request.NotifyPromo;
            user.NotifySecurity = request.NotifySecurity;
            RefreshReviewsProfile(db, user);

            db.SaveChanges();
            return (UpdateUserResult.Success, _mapper.Map<UserDto>(user));
        }

        // Смена пароля
        public ChangePasswordResult ChangePassword(int id, ChangePasswordRequest request)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return ChangePasswordResult.NotFound;

            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return ChangePasswordResult.InvalidNewPassword;
            }

            if (!PasswordHelper.Verify(request.CurrentPassword, user.Password))
                return ChangePasswordResult.InvalidCurrentPassword;

            var newPassword = request.NewPassword.Trim();

            if (newPassword.Length < 8)
                return ChangePasswordResult.InvalidNewPassword;

            if (PasswordHelper.Verify(newPassword, user.Password))
                return ChangePasswordResult.SamePassword;

            user.Password = PasswordHelper.Hash(newPassword);
            db.SaveChanges();
            return ChangePasswordResult.Success;
        }

        // Обновление аватара
        public (UpdateAvatarResult Result, UserDto? User) UpdateAvatar(int id, string avatarUrl)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return (UpdateAvatarResult.NotFound, null);

            user.AvatarUrl = avatarUrl.Trim();
            RefreshReviewsProfile(db, user);

            db.SaveChanges();
            return (UpdateAvatarResult.Success, _mapper.Map<UserDto>(user));
        }

        // Удаление
        public bool Delete(int id)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            db.Orders.RemoveRange(db.Orders.Where(o => o.UserId == id));
            db.Users.Remove(user);
            db.SaveChanges();
            return true;
        }

        // Бан
        public UserDto? SetBan(int id, bool isBanned)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            user.IsBanned = isBanned;
            db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }

        // Роль
        public UserDto? SetRole(int id, string role)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            user.Role = role;
            db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }

        private static void RefreshReviewsProfile(AppDbContext db, User user)
        {
            var avatar = BuildAvatar(user);
            var reviews = db.Reviews.Where(r => r.UserId == user.Id).ToList();

            foreach (var review in reviews)
            {
                review.Name = user.Username;
                review.Avatar = avatar;
            }
        }

        private static string BuildAvatar(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
                return user.AvatarUrl.Trim();

            var username = user.Username == null ? "" : user.Username.Trim();
            if (username == "")
                return "U";

            var parts = username.Split(' ');
            var first = "";
            var second = "";

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] != "")
                {
                    first = parts[i];
                    break;
                }
            }

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] != "" && parts[i] != first)
                {
                    second = parts[i];
                    break;
                }
            }

            if (first != "" && second != "")
                return (first.Substring(0, 1) + second.Substring(0, 1)).ToUpper();

            if (username.Length >= 2)
                return username.Substring(0, 2).ToUpper();

            return username.ToUpper();
        }
    }

    public enum UpdateUserResult
    {
        Success,
        NotFound,
        UsernameTaken,
        InvalidUsername
    }

    public enum ChangePasswordResult
    {
        Success,
        NotFound,
        InvalidCurrentPassword,
        InvalidNewPassword,
        SamePassword
    }

    public enum UpdateAvatarResult
    {
        Success,
        NotFound
    }
}
