namespace EgorkaCoins.Helpers.DTOs
{
    public class CreatePaymentRequest
    {
        public string Method { get; set; } = string.Empty;
        public List<CreatePaymentItemRequest> Items { get; set; } = new();
        public CardPaymentRequest? Card { get; set; }
        public CryptoPaymentRequest? Crypto { get; set; }
    }

    public class CreatePaymentItemRequest
    {
        public string PackageId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class CardPaymentRequest
    {
        public string Cardholder { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Expiry { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
    }

    public class CryptoPaymentRequest
    {
        public string Currency { get; set; } = string.Empty;
        public string Network { get; set; } = string.Empty;
        public string TxHash { get; set; } = string.Empty;
    }
}
