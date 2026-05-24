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
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentItem> PaymentItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Конструктор для DI
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Пустой конструктор для ручного создания
        public AppDbContext()
        {
        }

        // Настраиваем подключение
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbSession.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Игра -> пакеты
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Packages)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Пользователь -> заказы
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Пользователь -> платежи
            modelBuilder.Entity<User>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Платеж -> позиции
            modelBuilder.Entity<Payment>()
                .HasMany(p => p.Items)
                .WithOne(i => i.Payment)
                .HasForeignKey(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentItem>()
                .Property(i => i.TotalPrice)
                .HasPrecision(18, 2);

            // Пользователь -> отзывы
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
