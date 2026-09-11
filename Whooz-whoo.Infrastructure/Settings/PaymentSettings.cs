namespace Whooz_whoo.Infrastructure.Settings
{

    
    public class PaymentSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string Currency { get; set; } = "ZAR";
        public Dictionary<string, string> PriceIds { get; set; } = [];
    }
}
