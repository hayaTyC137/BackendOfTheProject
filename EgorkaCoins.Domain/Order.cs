using System.ComponentModel.DataAnnotations;

namespace EgorkaCoins.Domain
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string GameColor { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; }
    }
}