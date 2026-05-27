namespace EgorkaCoins.Helpers.DTOs
{
    public class LoginRequest
    {
        // Можно логиниться по email или нику
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
