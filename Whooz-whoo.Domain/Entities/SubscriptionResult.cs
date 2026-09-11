using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class SubscriptionResult
    {
        public bool IsSuccess { get; private set; }
        public string SubscriptionId { get; private set; } = string.Empty;
        public string CustomerId { get; private set; } = string.Empty;
        public string? PaymentIntentId { get; private set; }
        public string? ClientSecret { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public string? ErrorMessage { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }

        private SubscriptionResult()
        {
            Metadata = new Dictionary<string, string>();
        }

        public static SubscriptionResult Success(
            string subscriptionId,
            string customerId,
            string? paymentIntentId,
            string? clientSecret,
            DateTime startDate,
            DateTime endDate,
            decimal amount,
            string currency)
        {
            return new SubscriptionResult
            {
                IsSuccess = true,
                SubscriptionId = subscriptionId,
                CustomerId = customerId,
                PaymentIntentId = paymentIntentId,
                ClientSecret = clientSecret,
                StartDate = startDate,
                EndDate = endDate,
                Amount = amount,
                Currency = currency,
                Metadata = new Dictionary<string, string>()
            };
        }

        public static SubscriptionResult Failure(string errorMessage)
        {
            return new SubscriptionResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Metadata = new Dictionary<string, string>()
            };
        }
    }
}
