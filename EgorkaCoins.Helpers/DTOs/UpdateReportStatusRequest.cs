namespace EgorkaCoins.Helpers.DTOs
{
    public class UpdateReportStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string ModeratorComment { get; set; } = string.Empty;
    }
}
