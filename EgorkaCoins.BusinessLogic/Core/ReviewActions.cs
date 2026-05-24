using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class ReviewActions
    {
        // Все отзывы
        public List<Review> GetAll()
        {
            using var db = new AppDbContext();
            return db.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        // Отзыв по id
        public Review? GetById(int id)
        {
            using var db = new AppDbContext();
            return db.Reviews.FirstOrDefault(r => r.Id == id);
        }

        // Отзывы пользователя
        public List<Review> GetByUserId(int userId)
        {
            using var db = new AppDbContext();
            return db.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        // Создание отзыва
        public (ReviewActionResult Result, Review? Review) CreateForUser(CreateReviewRequest request, int userId)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null || user.IsBanned)
                return (ReviewActionResult.Forbidden, null);

            var review = new Review
            {
                UserId = user.Id,
                Name = user.Username,
                Avatar = BuildAvatar(user),
                CreatedAt = DateTime.UtcNow
            };

            ApplyReviewData(review, request);
            db.Reviews.Add(review);
            db.SaveChanges();
            return (ReviewActionResult.Success, review);
        }

        // Обновление отзыва
        public (ReviewActionResult Result, Review? Review) UpdateForUser(int id, CreateReviewRequest request, int userId)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null || user.IsBanned)
                return (ReviewActionResult.Forbidden, null);

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return (ReviewActionResult.NotFound, null);

            var canUpdate = review.UserId == userId ||
                user.Role == "admin" ||
                user.Role == "moderator";

            if (!canUpdate)
                return (ReviewActionResult.Forbidden, null);

            review.Name = user.Username;
            review.Avatar = BuildAvatar(user);
            ApplyReviewData(review, request);

            db.SaveChanges();
            return (ReviewActionResult.Success, review);
        }

        // Удаление своего отзыва
        public ReviewActionResult DeleteForUser(int id, int userId)
        {
            using var db = new AppDbContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null)
                return ReviewActionResult.NotFound;

            db.Reviews.Remove(review);
            db.SaveChanges();
            return ReviewActionResult.Success;
        }

        // Удаление отзыва по правам
        public ReviewActionResult DeleteAllowed(int id, int userId)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null || user.IsBanned)
                return ReviewActionResult.Forbidden;

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return ReviewActionResult.NotFound;

            var canDelete = review.UserId == userId ||
                user.Role == "admin" ||
                user.Role == "moderator";

            if (!canDelete)
                return ReviewActionResult.Forbidden;

            db.Reviews.Remove(review);
            db.SaveChanges();
            return ReviewActionResult.Success;
        }

        private static void ApplyReviewData(Review review, CreateReviewRequest request)
        {
            review.Game = request.Game.Trim();
            review.GameColor = string.IsNullOrEmpty(request.GameColor) ? "#B47AFF" : request.GameColor.Trim();
            review.Text = request.Text.Trim();
            review.Stars = request.Stars;
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

    public enum ReviewActionResult
    {
        Success,
        NotFound,
        Forbidden
    }
}
