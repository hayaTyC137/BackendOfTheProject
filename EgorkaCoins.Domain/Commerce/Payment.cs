using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EgorkaCoins.Domain
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public decimal TotalPrice { get; set; }
        public string? CardLast4 { get; set; }
        public string? CryptoCurrency { get; set; }
        public string? CryptoNetwork { get; set; }
        public string? CryptoTxHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public List<PaymentItem> Items { get; set; } = new();

        [JsonIgnore]
        public User? User { get; set; }
    }
}
