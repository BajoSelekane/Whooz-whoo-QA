using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class PaymentHistory : ValueObject
    {
        public string TransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string PaymentMethod { get; private set; }
        public string? FailureReason { get; private set; }

        public PaymentHistory(string transactionId, decimal amount, PaymentStatus status, DateTime paymentDate)
        {
            TransactionId = transactionId;
            Amount = amount;
            Status = status;
            PaymentDate = paymentDate;
        }

        public void MarkAsFailed(string reason)
        {
            Status = PaymentStatus.Failed;
            FailureReason = reason;
        }

        public void MarkAsRefunded()
        {
            Status = PaymentStatus.Refunded;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TransactionId;
            yield return PaymentDate;
        }
    }
}
