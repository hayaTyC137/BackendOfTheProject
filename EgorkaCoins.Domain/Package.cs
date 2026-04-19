namespace EgorkaCoins.Domain
{
    public class Package
    {
        public string Id { get; set; } = string.Empty;       // "vp1", "gem2" и т.д.
        public string GameId { get; set; } = string.Empty;   // связь с Game
        public int Amount { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Price { get; set; }                   // деньги — всегда decimal
        public decimal? OldPrice { get; set; }               
        public string? Bonus { get; set; }
        public string? Badge { get; set; }
        public bool Popular { get; set; }
    }
}
