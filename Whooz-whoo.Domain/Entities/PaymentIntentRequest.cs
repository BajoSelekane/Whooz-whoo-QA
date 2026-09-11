namespace Whooz_whoo.Domain.Entities
{
    public class PaymentIntentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string PaymentMethodId { get; set; } = string.Empty;
        public required string CustomerId { get; set; }
        public bool SetupFutureUsage { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public string? Description { get; set; }
        public string? StatementDescriptor { get; set; }
    }
}
