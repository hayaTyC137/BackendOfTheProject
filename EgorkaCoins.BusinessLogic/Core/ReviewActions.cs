using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class ReviewActions
    {
        // READ ALL
        public List<Review> GetAll()
        {
            using var db = new AppDbContext();
            return db.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        // READ ONE
        public Review? GetById(int id)
        {
            using var db = new AppDbContext();
            return db.Reviews.FirstOrDefault(r => r.Id == id);
        }

        // CREATE
        public Review Create(CreateReviewRequest request)
        {
            using var db = new AppDbContext();

            var review = new Review
            {
                Name = request.Name.Trim(),
                Game = request.Game.Trim(),
                GameColor = string.IsNullOrEmpty(request.GameColor) ? "#B47AFF" : request.GameColor.Trim(),
                Text = request.Text.Trim(),
                Stars = request.Stars,
                Avatar = request.Avatar.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Reviews.Add(review);
            db.SaveChanges();
            return review;
        }

        // UPDATE
        public Review? Update(int id, UpdateReviewRequest updated)
        {
            using var db = new AppDbContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null) return null;

            review.Name = updated.Name.Trim();
            review.Game = updated.Game.Trim();
            review.GameColor = string.IsNullOrEmpty(updated.GameColor) ? "#B47AFF" : updated.GameColor.Trim();
            review.Text = updated.Text.Trim();
            review.Stars = updated.Stars;
            review.Avatar = updated.Avatar.Trim();

            db.SaveChanges();
            return review;
        }

        // DELETE
        public bool Delete(int id)
        {
            using var db = new AppDbContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null) return false;

            db.Reviews.Remove(review);
            db.SaveChanges();
            return true;
        }
    }
}
