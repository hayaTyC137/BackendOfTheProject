using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class GameActions
    {
        // READ ALL
        public List<Game> GetAll()
        {
            using var db = new AppDbContext();
            return db.Games.ToList();
        }

        // READ ONE
        public Game? GetById(string id)
        {
            using var db = new AppDbContext();
            return db.Games.FirstOrDefault(g => g.Id == id);
        }

        // READ packages of a game
        public List<Package> GetPackages(string gameId)
        {
            using var db = new AppDbContext();
            return db.Packages.Where(p => p.GameId == gameId).ToList();
        }

        // CREATE
        public Game? Create(Game game)
        {
            using var db = new AppDbContext();

            var exists = db.Games.Any(g => g.Id == game.Id);
            if (exists) return null;

            db.Games.Add(game);
            db.SaveChanges();
            return game;
        }

        // UPDATE
        public Game? Update(string id, Game updated)
        {
            using var db = new AppDbContext();

            var game = db.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return null;

            game.Name = updated.Name;
            game.Currency = updated.Currency;
            game.Abbr = updated.Abbr;
            game.Color = updated.Color;
            game.Icon = updated.Icon;
            game.Description = updated.Description;
            game.Tag = updated.Tag;
            game.Banner = updated.Banner;
            game.About = updated.About;

            db.SaveChanges();
            return game;
        }

        // DELETE
        public bool Delete(string id)
        {
            using var db = new AppDbContext();

            var game = db.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return false;

            db.Games.Remove(game);
            db.SaveChanges();
            return true;
        }
    }
}