using Whooz_whoo.Domain.Entities;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(
            string paymentMethodId,
            decimal amount,
            bool isRecurring = false,
            string currency = "ZAR",
            Dictionary<string, string>? metadata = null);

        Task<PaymentResult> ProcessPaymentAsync(PaymentIntentRequest request);

        Task<RefundResult> RefundPaymentAsync(
            string transactionId,
            decimal? amount = null,
            string? reason = null);

        Task<SubscriptionResult> CreateSubscriptionAsync(
            string customerId,
            string priceId,
            string? paymentMethodId = null,
            Dictionary<string, string>? metadata = null);

        Task<bool> CancelSubscriptionAsync(
            string subscriptionId,
            bool immediately = false);

        Task<SubscriptionResult> UpdateSubscriptionAsync(
            string subscriptionId,
            string newPriceId,
            string? paymentMethodId = null);

        Task<bool> PauseSubscriptionAsync(
            string subscriptionId,
            int pauseDays);

        Task<bool> ResumeSubscriptionAsync(string subscriptionId);

        Task<DateTime?> GetSubscriptionEndDateAsync(string subscriptionId);

        Task<CustomerResult> CreateCustomerAsync(
            string email,
            string name,
            string? phoneNumber = null,
            string? paymentMethodId = null,
            Dictionary<string, string>? metadata = null);

        Task<CustomerResult> GetCustomerAsync(string customerId);

        Task<CustomerResult> UpdateCustomerAsync(
            string customerId,
            string? email = null,
            string? name = null,
            string? phoneNumber = null,
            Dictionary<string, string>? metadata = null);

        Task<bool> DeleteCustomerAsync(string customerId);

        Task<PaymentMethodDetails?> CreatePaymentMethodAsync(
            string paymentMethodId,
            bool setAsDefault = false);

        Task<PaymentMethodDetails?> GetPaymentMethodAsync(string paymentMethodId);

        Task<bool> DeletePaymentMethodAsync(string paymentMethodId);

        Task<PaymentMethodDetails?> SetDefaultPaymentMethodAsync(
            string customerId,
            string paymentMethodId);

        Task<List<PaymentMethodDetails>> GetCustomerPaymentMethodsAsync(string customerId);

        Task<bool> ValidatePaymentMethodAsync(string paymentMethodId);

        Task<bool> ValidatePaymentAsync(
            string transactionId,
            decimal expectedAmount = 0);

        Task<PaymentWebhookEvent> HandleWebhookAsync(string payload, string signature);

        Task<PaymentStatus> GetPaymentStatusAsync(string transactionId);

        Task<PaymentResult> GetPaymentAsync(string transactionId);

        Task<string?> GetPaymentIntentClientSecretAsync(string transactionId);
    }
}
