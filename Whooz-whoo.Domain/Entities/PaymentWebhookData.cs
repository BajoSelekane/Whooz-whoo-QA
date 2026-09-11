namespace Whooz_whoo.Domain.Entities
{
    public class PaymentWebhookData
    {
        public string Object { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}
