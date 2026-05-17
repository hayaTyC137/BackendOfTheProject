namespace EgorkaCoins.Helpers.DTOs
{
    public class CreateReviewRequest
    {
        public string Game { get; set; } = string.Empty;
        public string GameColor { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Stars { get; set; }
    }
}
