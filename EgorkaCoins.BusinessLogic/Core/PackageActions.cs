using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class PackageActions
    {
        // READ ALL
        public List<Package> GetAll()
        {
            using var db = new AppDbContext();
            return db.Packages.ToList();
        }

        // READ ONE
        public Package? GetById(string id)
        {
            using var db = new AppDbContext();
            return db.Packages.FirstOrDefault(p => p.Id == id);
        }

        // READ BY GAME
        public List<Package> GetByGame(string gameId)
        {
            using var db = new AppDbContext();
            return db.Packages.Where(p => p.GameId == gameId).ToList();
        }

        // CREATE
        public Package? Create(Package package)
        {
            using var db = new AppDbContext();

            var exists = db.Packages.Any(p => p.Id == package.Id);
            if (exists) return null;

            db.Packages.Add(package);
            db.SaveChanges();
            return package;
        }

        // UPDATE
        public Package? Update(string id, Package updated)
        {
            using var db = new AppDbContext();

            var package = db.Packages.FirstOrDefault(p => p.Id == id);
            if (package == null) return null;

            package.Label = updated.Label;
            package.Amount = updated.Amount;
            package.Price = updated.Price;
            package.OldPrice = updated.OldPrice;
            package.Bonus = updated.Bonus;
            package.Badge = updated.Badge;
            package.Popular = updated.Popular;

            db.SaveChanges();
            return package;
        }

        // DELETE
        public bool Delete(string id)
        {
            using var db = new AppDbContext();

            var package = db.Packages.FirstOrDefault(p => p.Id == id);
            if (package == null) return false;

            db.Packages.Remove(package);
            db.SaveChanges();
            return true;
        }
    }
}