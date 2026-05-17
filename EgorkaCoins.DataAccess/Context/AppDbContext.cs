using EgorkaCoins.Domain;
using Microsoft.EntityFrameworkCore;

namespace EgorkaCoins.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Report> Reports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Конструктор с параметрами — для Program.cs (Dependency Injection)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Пустой конструктор — для UserActions (new AppDbContext())
        public AppDbContext()
        {
        }

        // Настройка подключения через DbSession
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbSession.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Game 1:N Package
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Packages)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1:N Order
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1:N Review.
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
