using System.Text.Json.Serialization;

namespace EgorkaCoins.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal TotalSpent { get; set; }
        public int OrdersCount { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Xp { get; set; }
        public int XpToNext { get; set; }
        public bool Verified { get; set; }
        public bool IsBanned { get; set; }
        public bool NotifyOrders { get; set; }
        public bool NotifyPromo { get; set; }
        public bool NotifySecurity { get; set; }
        public DateTime CreatedAt { get; set; }

        // Список заказов
        [JsonIgnore]
        public List<Order> Orders { get; set; } = new();

        [JsonIgnore]
        public List<Payment> Payments { get; set; } = new();

        [JsonIgnore]
        public List<Review> Reviews { get; set; } = new();
    }
}
