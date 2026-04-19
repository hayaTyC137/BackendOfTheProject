namespace EgorkaCoins.Helpers.DTOs
{
    public class LoginRequest
    {
        // identifier может быть email или username — как на твоём фронте
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}