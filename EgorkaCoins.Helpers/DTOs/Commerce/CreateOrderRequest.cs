namespace EgorkaCoins.Helpers.DTOs
{
    public class CreateOrderRequest
    {
        public string GameName { get; set; } = string.Empty;
        public string GameColor { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}