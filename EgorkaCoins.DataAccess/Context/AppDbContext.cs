using EgorkaCoins.Domain;
using Microsoft.EntityFrameworkCore;

namespace EgorkaCoins.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Конструктор с параметрами — для Program.cs (Dependency Injection)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Пустой конструктор — для UserActions (new AppDbContext())
        public AppDbContext()
        {
        }

        // Настройка подключения через DbSession — как в документе преподавателя
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbSession.ConnectionString);
            }
        }
    }
}