using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EgorkaCoins.Domain
{
    public class PaymentItem
    {
        [Key]
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string PackageId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public string GameColor { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        [JsonIgnore]
        public Payment? Payment { get; set; }
    }
}
