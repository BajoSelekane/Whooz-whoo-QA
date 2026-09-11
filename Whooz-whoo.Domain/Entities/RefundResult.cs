using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class RefundResult
    {
        public bool IsSuccess { get; private set; }
        public string RefundId { get; private set; } = string.Empty;
        public string TransactionId { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public DateTime RefundedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }

        private RefundResult()
        {
            Metadata = new Dictionary<string, string>();
        }

        public static RefundResult Success(
            string refundId,
            string transactionId,
            decimal amount,
            string currency)
        {
            return new RefundResult
            {
                IsSuccess = true,
                RefundId = refundId,
                TransactionId = transactionId,
                Amount = amount,
                Currency = currency,
                RefundedAt = DateTime.UtcNow,
                Metadata = new Dictionary<string, string>()
            };
        }

        public static RefundResult Failure(string errorMessage)
        {
            return new RefundResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Metadata = new Dictionary<string, string>()
            };
        }
    }
}
