using System.ComponentModel.DataAnnotations;

namespace EgorkaCoins.Domain
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public string GameColor { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
