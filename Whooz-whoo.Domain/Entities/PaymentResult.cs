using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class PaymentResult
    {
        public bool IsSuccess { get; private set; }
        public string TransactionId { get; private set; } = string.Empty;
        public string PaymentIntentId { get; private set; } = string.Empty;
        public string ClientSecret { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public PaymentStatus Status { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? ErrorCode { get; private set; }
        public DateTime ProcessedAt { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; } = [];
        public PaymentMethodDetails? PaymentMethodDetails { get; private set; }

        private PaymentResult()
        {
            Metadata = [];
        }

        public static PaymentResult Success(
            string transactionId,
            string paymentIntentId,
            string clientSecret,
            decimal amount,
            string currency,
            PaymentMethodDetails paymentMethodDetails)
        {
            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = transactionId,
                PaymentIntentId = paymentIntentId,
                ClientSecret = clientSecret,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Completed,
                ProcessedAt = DateTime.UtcNow,
                PaymentMethodDetails = paymentMethodDetails,
                Metadata = []
            };
        }

        public static PaymentResult Failure(string errorMessage, string? errorCode = null)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                Status = PaymentStatus.Failed,
                ProcessedAt = DateTime.UtcNow,
                Metadata = []
            };
        }

        public static PaymentResult Pending(
            string transactionId,
            string paymentIntentId,
            string clientSecret,
            decimal amount,
            string currency)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                TransactionId = transactionId,
                PaymentIntentId = paymentIntentId,
                ClientSecret = clientSecret,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Pending,
                ProcessedAt = DateTime.UtcNow,
                Metadata = []
            };
        }

        public void AddMetadata(string key, string value)
        {
            Metadata[key] = value;
        }
    }
}
