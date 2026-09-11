using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class PaymentMethodDetails
    {
        public string PaymentMethodId { get; private set; } = string.Empty;
        public PaymentMethodType Type { get; private set; }
        public string? Last4Digits { get; private set; }
        public string? Brand { get; private set; }
        public string? ExpiryMonth { get; private set; }
        public string? ExpiryYear { get; private set; }
        public string? Fingerprint { get; private set; }
        public bool IsDefault { get; private set; }

        private PaymentMethodDetails() { }

        public PaymentMethodDetails(
            string paymentMethodId,
            PaymentMethodType type,
            string? last4Digits = null,
            string? brand = null,
            string? expiryMonth = null,
            string? expiryYear = null)
        {
            PaymentMethodId = paymentMethodId;
            Type = type;
            Last4Digits = last4Digits;
            Brand = brand;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            IsDefault = false;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
        }

        public string GetMaskedCardNumber()
        {
            if (string.IsNullOrEmpty(Last4Digits))
                return "•••• •••• •••• ••••";

            return $"•••• •••• •••• {Last4Digits}";
        }
    }
}
