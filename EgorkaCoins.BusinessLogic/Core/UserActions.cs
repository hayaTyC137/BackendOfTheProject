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

        public UserDto? Register(RegisterRequest request)
        {
            using var db = new AppDbContext();

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

            return _mapper.Map<UserDto>(user);
        }

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

        public UserDto? GetById(int id)
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public List<UserDto> GetAll()
        {
            using var db = new AppDbContext();
            return db.Users.Select(u => _mapper.Map<UserDto>(u)).ToList();
        }

        public UserDto? Update(int id, UpdateUserRequest request)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            if (user.Username != request.Username)
            {
                var taken = db.Users.Any(u => u.Username == request.Username && u.Id != id);
                if (taken) return null;
            }

            if (user.Email != request.Email.ToLower())
            {
                var taken = db.Users.Any(u => u.Email == request.Email.ToLower() && u.Id != id);
                if (taken) return null;
            }

            user.Username = request.Username.Trim();
            user.Email = request.Email.Trim().ToLower();

            db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }

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

        public UserDto? SetBan(int id, bool isBanned)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            user.IsBanned = isBanned;
            db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }

        public UserDto? SetRole(int id, string role)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            user.Role = role;
            db.SaveChanges();
            return _mapper.Map<UserDto>(user);
        }
    }
}