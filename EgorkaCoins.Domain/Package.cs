namespace EgorkaCoins.Domain
{
    public class Package
    {
        public string Id { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Bonus { get; set; }
        public string? Badge { get; set; }
        public bool Popular { get; set; }

        // Обратная навигация N:1
        public Game? Game { get; set; }
    }
}
