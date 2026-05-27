namespace EgorkaCoins.Helpers.DTOs
{
    public class UpdateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public bool NotifyOrders { get; set; }
        public bool NotifyPromo { get; set; }
        public bool NotifySecurity { get; set; }
    }
}
