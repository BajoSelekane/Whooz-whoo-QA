using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class CustomerResult
    {
        public bool IsSuccess { get; private set; }
        public string CustomerId { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public string? DefaultPaymentMethodId { get; private set; }
        public List<PaymentMethodDetails> PaymentMethods { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }
        public string? ErrorMessage { get; private set; }

        private CustomerResult()
        {
            PaymentMethods = new List<PaymentMethodDetails>();
            Metadata = new Dictionary<string, string>();
        }

        public static CustomerResult Success(
            string customerId,
            string email,
            string name,
            string? defaultPaymentMethodId = null)
        {
            return new CustomerResult
            {
                IsSuccess = true,
                CustomerId = customerId,
                Email = email,
                Name = name,
                DefaultPaymentMethodId = defaultPaymentMethodId,
                PaymentMethods = new List<PaymentMethodDetails>(),
                Metadata = new Dictionary<string, string>()
            };
        }

        public static CustomerResult Failure(string errorMessage)
        {
            return new CustomerResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                PaymentMethods = new List<PaymentMethodDetails>(),
                Metadata = new Dictionary<string, string>()
            };
        }

        public void AddPaymentMethod(PaymentMethodDetails paymentMethod)
        {
            PaymentMethods.Add(paymentMethod);
        }
    }
}
