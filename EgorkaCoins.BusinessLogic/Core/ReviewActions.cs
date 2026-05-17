using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class ReviewActions
    {
        public List<Review> GetAll()
        {
            using var db = new AppDbContext();
            return db.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public Review? GetById(int id)
        {
            using var db = new AppDbContext();
            return db.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetByUserId(int userId)
        {
            using var db = new AppDbContext();
            return db.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

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
                Avatar = BuildAvatar(user.Username),
                CreatedAt = DateTime.UtcNow
            };

            ApplyReviewData(review, request);
            db.Reviews.Add(review);
            db.SaveChanges();
            return (ReviewActionResult.Success, review);
        }

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

            ApplyReviewData(review, request);

            db.SaveChanges();
            return (ReviewActionResult.Success, review);
        }

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

        private static string BuildAvatar(string username)
        {
            var trimmed = username.Trim();
            if (string.IsNullOrEmpty(trimmed)) return "U";

            var parts = trimmed
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            return trimmed.Length >= 2
                ? trimmed[..2].ToUpper()
                : trimmed.ToUpper();
        }
    }

    public enum ReviewActionResult
    {
        Success,
        NotFound,
        Forbidden
    }
}
