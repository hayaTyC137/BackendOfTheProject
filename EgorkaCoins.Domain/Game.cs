using System.ComponentModel.DataAnnotations;

namespace EgorkaCoins.Domain
{
    public class Game
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Abbr { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
    }
}