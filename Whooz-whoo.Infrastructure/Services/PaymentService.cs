using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Whooz_whoo.Domain.Entities;
using Whooz_whoo.Domain.Enums;
using Whooz_whoo.Domain.Interfaces;
using Whooz_whoo.Infrastructure.Settings;

namespace Whooz_whoo.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<PaymentSettings> _settings;
        private readonly ICacheService _cacheService;
        private readonly ILogger<PaymentService> _logger;
        private readonly string _apiKey;

        public PaymentService(
            HttpClient httpClient,
            IOptions<PaymentSettings> settings,
            ICacheService cacheService,
            ILogger<PaymentService> logger)
        {
            _httpClient = httpClient;
            _settings = settings;
            _cacheService = cacheService;
            _logger = logger;
            _apiKey = settings.Value.ApiKey;

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<PaymentResult> ProcessPaymentAsync(
            string paymentMethodId,
            decimal amount,
            bool isRecurring = false,
            string currency = "ZAR",
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                var request = new
                {
                    payment_method_id = paymentMethodId,
                    amount,
                    currency,
                    capture_method = "automatic",
                    confirmation_method = "automatic",
                    setup_future_usage = isRecurring ? "off_session" : "on_session",
                    metadata = metadata ?? []
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/payment_intents", request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ToPaymentFailure(content, "Payment processing failed");

                var result = JsonSerializer.Deserialize<PaymentIntentResponse>(content);
                if (result is null)
                    return PaymentResult.Failure("Payment processing failed");

                await _cacheService.SetAsync(
                    $"payment:{result.Id}",
                    result,
                    TimeSpan.FromMinutes(5));

                return PaymentResult.Success(
                    result.Id,
                    result.Id,
                    result.ClientSecret,
                    result.Amount,
                    result.Currency,
                    new PaymentMethodDetails(
                        paymentMethodId,
                        PaymentMethodType.Card,
                        result.Last4Digits,
                        result.Brand
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return PaymentResult.Failure($"Payment processing failed: {ex.Message}");
            }
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentIntentRequest request)
        {
            return await ProcessPaymentAsync(
                request.PaymentMethodId,
                request.Amount,
                request.SetupFutureUsage,
                request.Currency,
                request.Metadata);
        }

        public async Task<RefundResult> RefundPaymentAsync(
            string transactionId,
            decimal? amount = null,
            string? reason = null)
        {
            try
            {
                var refundRequest = new
                {
                    payment_intent = transactionId,
                    amount,
                    reason = reason ?? "requested_by_customer",
                    metadata = new Dictionary<string, string>
                    {
                        { "refund_reason", reason ?? "Customer requested refund" }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/refunds", refundRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return RefundResult.Failure(ReadErrorMessage(content, "Refund processing failed"));

                var result = JsonSerializer.Deserialize<RefundResponse>(content);
                if (result is null)
                    return RefundResult.Failure("Refund processing failed");

                await _cacheService.RemoveAsync($"payment:{transactionId}");

                return RefundResult.Success(
                    result.Id,
                    result.PaymentIntentId,
                    result.Amount,
                    result.Currency
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for transaction {TransactionId}", transactionId);
                return RefundResult.Failure($"Refund processing failed: {ex.Message}");
            }
        }

        public async Task<SubscriptionResult> CreateSubscriptionAsync(
            string customerId,
            string priceId,
            string? paymentMethodId = null,
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                var subscriptionRequest = new
                {
                    customer = customerId,
                    items = new[] { new { price = priceId } },
                    default_payment_method = paymentMethodId,
                    payment_behavior = "default_incomplete",
                    expand = new[] { "latest_invoice.payment_intent" },
                    metadata = metadata ?? []
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/subscriptions", subscriptionRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return SubscriptionResult.Failure(ReadErrorMessage(content, "Subscription creation failed"));

                var result = JsonSerializer.Deserialize<SubscriptionResponse>(content);
                if (result is null)
                    return SubscriptionResult.Failure("Subscription creation failed");

                return ToSubscriptionResult(result, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for customer {CustomerId}", customerId);
                return SubscriptionResult.Failure($"Subscription creation failed: {ex.Message}");
            }
        }

        public async Task<bool> CancelSubscriptionAsync(
            string subscriptionId,
            bool immediately = false)
        {
            try
            {
                var cancellationRequest = new
                {
                    cancel_at_period_end = !immediately,
                    metadata = new Dictionary<string, string>
                    {
                        { "cancellation_reason", "User requested cancellation" }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/subscriptions/{subscriptionId}",
                    cancellationRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to cancel subscription {SubscriptionId}: {Error}",
                        subscriptionId, ReadErrorMessage(content, "Cancellation failed"));
                    return false;
                }

                await _cacheService.RemoveAsync($"subscription:{subscriptionId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription {SubscriptionId}", subscriptionId);
                return false;
            }
        }

        public async Task<SubscriptionResult> UpdateSubscriptionAsync(
            string subscriptionId,
            string newPriceId,
            string? paymentMethodId = null)
        {
            try
            {
                var updateRequest = new
                {
                    items = new[]
                    {
                        new
                        {
                            id = subscriptionId,
                            price = newPriceId
                        }
                    },
                    default_payment_method = paymentMethodId,
                    proration_behavior = "create_prorations"
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/subscriptions/{subscriptionId}",
                    updateRequest);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return SubscriptionResult.Failure(ReadErrorMessage(content, "Subscription update failed"));

                var result = JsonSerializer.Deserialize<SubscriptionResponse>(content);
                if (result is null)
                    return SubscriptionResult.Failure("Subscription update failed");

                await _cacheService.RemoveAsync($"subscription:{subscriptionId}");

                return ToSubscriptionResult(result, result.StartDate, result.EndDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription {SubscriptionId}", subscriptionId);
                return SubscriptionResult.Failure($"Subscription update failed: {ex.Message}");
            }
        }

        public async Task<bool> PauseSubscriptionAsync(
            string subscriptionId,
            int pauseDays)
        {
            try
            {
                var pauseRequest = new
                {
                    pause_collection = new
                    {
                        behavior = "void",
                        resumes_at = new DateTimeOffset(DateTime.UtcNow.AddDays(pauseDays)).ToUnixTimeSeconds()
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/subscriptions/{subscriptionId}",
                    pauseRequest);

                if (!response.IsSuccessStatusCode)
                    return false;

                await _cacheService.RemoveAsync($"subscription:{subscriptionId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pausing subscription {SubscriptionId}", subscriptionId);
                return false;
            }
        }

        public async Task<bool> ResumeSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var resumeRequest = new
                {
                    pause_collection = new
                    {
                        behavior = "keep_as_draft"
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/subscriptions/{subscriptionId}",
                    resumeRequest);

                if (!response.IsSuccessStatusCode)
                    return false;

                await _cacheService.RemoveAsync($"subscription:{subscriptionId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming subscription {SubscriptionId}", subscriptionId);
                return false;
            }
        }

        public async Task<DateTime?> GetSubscriptionEndDateAsync(string subscriptionId)
        {
            try
            {
                var cacheKey = $"subscription_end:{subscriptionId}";
                var cached = await _cacheService.GetAsync<CachedDateTime>(cacheKey);
                if (cached != null)
                    return cached.Value;

                var response = await _httpClient.GetAsync($"/v1/subscriptions/{subscriptionId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SubscriptionResponse>(content);
                if (result is null)
                    return null;

                await _cacheService.SetAsync(cacheKey, new CachedDateTime { Value = result.EndDate }, TimeSpan.FromMinutes(30));
                return result.EndDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription end date for {SubscriptionId}", subscriptionId);
                return null;
            }
        }

        public async Task<CustomerResult> CreateCustomerAsync(
            string email,
            string name,
            string? phoneNumber = null,
            string? paymentMethodId = null,
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                var customerRequest = new
                {
                    email,
                    name,
                    phone = phoneNumber,
                    default_payment_method = paymentMethodId,
                    metadata = metadata ?? []
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/customers", customerRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return CustomerResult.Failure(ReadErrorMessage(content, "Customer creation failed"));

                var result = JsonSerializer.Deserialize<CustomerResponse>(content);
                if (result is null)
                    return CustomerResult.Failure("Customer creation failed");

                return ToCustomerResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer {Email}", email);
                return CustomerResult.Failure($"Customer creation failed: {ex.Message}");
            }
        }

        public async Task<CustomerResult> GetCustomerAsync(string customerId)
        {
            try
            {
                var cacheKey = $"customer:{customerId}";
                var cached = await _cacheService.GetAsync<CustomerResult>(cacheKey);
                if (cached != null)
                    return cached;

                var response = await _httpClient.GetAsync($"/v1/customers/{customerId}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return CustomerResult.Failure("Customer not found");

                var result = JsonSerializer.Deserialize<CustomerResponse>(content);
                if (result is null)
                    return CustomerResult.Failure("Customer not found");

                var customerResult = ToCustomerResult(result);

                await _cacheService.SetAsync(cacheKey, customerResult, TimeSpan.FromMinutes(10));
                return customerResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId}", customerId);
                return CustomerResult.Failure($"Failed to get customer: {ex.Message}");
            }
        }

        public async Task<CustomerResult> UpdateCustomerAsync(
            string customerId,
            string? email = null,
            string? name = null,
            string? phoneNumber = null,
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                var updateRequest = new
                {
                    email,
                    name,
                    phone = phoneNumber,
                    metadata
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/customers/{customerId}",
                    updateRequest);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return CustomerResult.Failure("Customer update failed");

                var result = JsonSerializer.Deserialize<CustomerResponse>(content);
                if (result is null)
                    return CustomerResult.Failure("Customer update failed");

                await _cacheService.RemoveAsync($"customer:{customerId}");

                return ToCustomerResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", customerId);
                return CustomerResult.Failure($"Customer update failed: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/v1/customers/{customerId}");

                if (!response.IsSuccessStatusCode)
                    return false;

                await _cacheService.RemoveAsync($"customer:{customerId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", customerId);
                return false;
            }
        }

        public async Task<PaymentMethodDetails?> CreatePaymentMethodAsync(
            string paymentMethodId,
            bool setAsDefault = false)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v1/payment_methods/{paymentMethodId}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = JsonSerializer.Deserialize<PaymentMethodResponse>(content);
                if (result is null)
                    return null;

                var paymentMethod = ToPaymentMethodDetails(result);

                if (setAsDefault)
                    paymentMethod.SetAsDefault();

                return paymentMethod;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment method {PaymentMethodId}", paymentMethodId);
                return null;
            }
        }

        public async Task<PaymentMethodDetails?> GetPaymentMethodAsync(string paymentMethodId)
        {
            try
            {
                var cacheKey = $"payment_method:{paymentMethodId}";
                var cached = await _cacheService.GetAsync<PaymentMethodDetails>(cacheKey);
                if (cached != null)
                    return cached;

                var response = await _httpClient.GetAsync($"/v1/payment_methods/{paymentMethodId}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = JsonSerializer.Deserialize<PaymentMethodResponse>(content);
                if (result is null)
                    return null;

                var paymentMethod = ToPaymentMethodDetails(result);

                await _cacheService.SetAsync(cacheKey, paymentMethod, TimeSpan.FromMinutes(30));
                return paymentMethod;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment method {PaymentMethodId}", paymentMethodId);
                return null;
            }
        }

        public async Task<bool> DeletePaymentMethodAsync(string paymentMethodId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/v1/payment_methods/{paymentMethodId}");

                if (!response.IsSuccessStatusCode)
                    return false;

                await _cacheService.RemoveAsync($"payment_method:{paymentMethodId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment method {PaymentMethodId}", paymentMethodId);
                return false;
            }
        }

        public async Task<PaymentMethodDetails?> SetDefaultPaymentMethodAsync(
            string customerId,
            string paymentMethodId)
        {
            try
            {
                var updateRequest = new
                {
                    default_payment_method = paymentMethodId
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"/v1/customers/{customerId}",
                    updateRequest);

                if (!response.IsSuccessStatusCode)
                    return null;

                await _cacheService.RemoveAsync($"customer:{customerId}");
                await _cacheService.RemoveAsync($"customer_payment_methods:{customerId}");

                return await GetPaymentMethodAsync(paymentMethodId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default payment method {PaymentMethodId}", paymentMethodId);
                return null;
            }
        }

        public async Task<List<PaymentMethodDetails>> GetCustomerPaymentMethodsAsync(string customerId)
        {
            try
            {
                var cacheKey = $"customer_payment_methods:{customerId}";
                var cached = await _cacheService.GetAsync<List<PaymentMethodDetails>>(cacheKey);
                if (cached != null)
                    return cached;

                var response = await _httpClient.GetAsync(
                    $"/v1/payment_methods?customer={customerId}&type=card");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return [];

                var result = JsonSerializer.Deserialize<PaymentMethodListResponse>(content);
                if (result?.Data is null)
                    return [];

                var paymentMethods = result.Data.Select(ToPaymentMethodDetails).ToList();

                await _cacheService.SetAsync(cacheKey, paymentMethods, TimeSpan.FromMinutes(15));
                return paymentMethods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer payment methods {CustomerId}", customerId);
                return [];
            }
        }

        public async Task<bool> ValidatePaymentMethodAsync(string paymentMethodId)
        {
            try
            {
                var paymentMethod = await GetPaymentMethodAsync(paymentMethodId);
                return paymentMethod != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidatePaymentAsync(
            string transactionId,
            decimal expectedAmount = 0)
        {
            try
            {
                var payment = await GetPaymentAsync(transactionId);
                if (!payment.IsSuccess)
                    return false;

                if (expectedAmount > 0 && payment.Amount != expectedAmount)
                    return false;

                return payment.Status == PaymentStatus.Completed;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PaymentWebhookEvent> HandleWebhookAsync(string payload, string signature)
        {
            try
            {
                var secret = _settings.Value.WebhookSecret;
                var isValid = VerifyWebhookSignature(payload, signature, secret);

                if (!isValid)
                    throw new UnauthorizedAccessException("Invalid webhook signature");

                var webhookEvent = JsonSerializer.Deserialize<PaymentWebhookEvent>(payload);
                if (webhookEvent is null)
                    throw new InvalidOperationException("Invalid webhook payload");

                await ProcessWebhookEvent(webhookEvent);

                return webhookEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling webhook");
                throw;
            }
        }

        public async Task<PaymentStatus> GetPaymentStatusAsync(string transactionId)
        {
            try
            {
                var payment = await GetPaymentAsync(transactionId);
                return payment.Status;
            }
            catch
            {
                return PaymentStatus.Failed;
            }
        }

        public async Task<PaymentResult> GetPaymentAsync(string transactionId)
        {
            try
            {
                var cacheKey = $"payment:{transactionId}";
                var cached = await _cacheService.GetAsync<PaymentResult>(cacheKey);
                if (cached != null)
                    return cached;

                var response = await _httpClient.GetAsync($"/v1/payment_intents/{transactionId}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return PaymentResult.Failure("Payment not found");

                var result = JsonSerializer.Deserialize<PaymentIntentResponse>(content);
                if (result is null)
                    return PaymentResult.Failure("Payment not found");

                var paymentResult = PaymentResult.Success(
                    result.Id,
                    result.Id,
                    result.ClientSecret,
                    result.Amount,
                    result.Currency,
                    new PaymentMethodDetails(
                        result.PaymentMethodId,
                        PaymentMethodType.Card,
                        result.Last4Digits,
                        result.Brand
                    )
                );

                await _cacheService.SetAsync(cacheKey, paymentResult, TimeSpan.FromMinutes(5));
                return paymentResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment {TransactionId}", transactionId);
                return PaymentResult.Failure($"Failed to get payment: {ex.Message}");
            }
        }

        public async Task<string?> GetPaymentIntentClientSecretAsync(string transactionId)
        {
            try
            {
                var payment = await GetPaymentAsync(transactionId);
                return payment.ClientSecret;
            }
            catch
            {
                return null;
            }
        }

        private static bool VerifyWebhookSignature(string payload, string signature, string secret)
        {
            // Implement webhook signature verification
            // This depends on the payment provider (Stripe, PayFast, etc.)
            _ = payload;
            _ = signature;
            _ = secret;
            return true;
        }

        private async Task ProcessWebhookEvent(PaymentWebhookEvent webhookEvent)
        {
            switch (webhookEvent.Type)
            {
                case "payment_intent.succeeded":
                    await HandlePaymentSucceeded(webhookEvent);
                    break;
                case "payment_intent.payment_failed":
                    await HandlePaymentFailed(webhookEvent);
                    break;
                case "payment_intent.refunded":
                    await HandlePaymentRefunded(webhookEvent);
                    break;
                case "customer.subscription.created":
                    await HandleSubscriptionCreated(webhookEvent);
                    break;
                case "customer.subscription.updated":
                    await HandleSubscriptionUpdated(webhookEvent);
                    break;
                case "customer.subscription.deleted":
                    await HandleSubscriptionCancelled(webhookEvent);
                    break;
                default:
                    _logger.LogInformation("Unhandled webhook event type: {EventType}", webhookEvent.Type);
                    break;
            }
        }

        private async Task HandlePaymentSucceeded(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogInformation("Payment succeeded: {PaymentId}", data.Id);
            await _cacheService.RemoveAsync($"payment:{data.Id}");
        }

        private async Task HandlePaymentFailed(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogWarning("Payment failed: {PaymentId}", data.Id);
            await _cacheService.RemoveAsync($"payment:{data.Id}");
        }

        private async Task HandlePaymentRefunded(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogInformation("Payment refunded: {PaymentId}", data.Id);
            await _cacheService.RemoveAsync($"payment:{data.Id}");
        }

        private async Task HandleSubscriptionCreated(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogInformation("Subscription created: {SubscriptionId}", data.Id);
            await _cacheService.RemoveAsync($"subscription_end:{data.Id}");
        }

        private async Task HandleSubscriptionUpdated(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogInformation("Subscription updated: {SubscriptionId}", data.Id);
            await _cacheService.RemoveAsync($"subscription_end:{data.Id}");
        }

        private async Task HandleSubscriptionCancelled(PaymentWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;
            if (data is null)
                return;

            _logger.LogInformation("Subscription cancelled: {SubscriptionId}", data.Id);
            await _cacheService.RemoveAsync($"subscription_end:{data.Id}");
        }

        private static PaymentResult ToPaymentFailure(string content, string fallback)
        {
            var error = JsonSerializer.Deserialize<PaymentError>(content);
            return PaymentResult.Failure(error?.Message ?? fallback, error?.Code);
        }

        private static string ReadErrorMessage(string content, string fallback)
        {
            var error = JsonSerializer.Deserialize<PaymentError>(content);
            return error?.Message ?? fallback;
        }

        private static SubscriptionResult ToSubscriptionResult(
            SubscriptionResponse result,
            DateTime startDate,
            DateTime endDate)
        {
            return SubscriptionResult.Success(
                result.Id,
                result.CustomerId,
                result.LatestInvoice?.PaymentIntentId,
                result.LatestInvoice?.PaymentIntent?.ClientSecret,
                startDate,
                endDate,
                result.LatestInvoice?.Amount ?? 0,
                result.Currency
            );
        }

        private static CustomerResult ToCustomerResult(CustomerResponse result)
        {
            return CustomerResult.Success(
                result.Id,
                result.Email,
                result.Name,
                result.DefaultPaymentMethodId
            );
        }

        private static PaymentMethodDetails ToPaymentMethodDetails(PaymentMethodResponse result)
        {
            return new PaymentMethodDetails(
                result.Id,
                PaymentMethodType.Card,
                result.Last4Digits,
                result.Brand,
                result.ExpiryMonth,
                result.ExpiryYear
            );
        }

        private class CachedDateTime
        {
            public DateTime Value { get; set; }
        }

        private class PaymentIntentResponse
        {
            public string Id { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string PaymentMethodId { get; set; } = string.Empty;
            public string? Last4Digits { get; set; }
            public string? Brand { get; set; }
            public Dictionary<string, string> Metadata { get; set; } = [];
        }

        private class RefundResponse
        {
            public string Id { get; set; } = string.Empty;
            public string PaymentIntentId { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }

        private class SubscriptionResponse
        {
            public string Id { get; set; } = string.Empty;
            public string CustomerId { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public InvoiceResponse? LatestInvoice { get; set; }
            public Dictionary<string, string> Metadata { get; set; } = [];
        }

        private class InvoiceResponse
        {
            public string Id { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string? PaymentIntentId { get; set; }
            public PaymentIntentResponse? PaymentIntent { get; set; }
        }

        private class CustomerResponse
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? DefaultPaymentMethodId { get; set; }
            public Dictionary<string, string> Metadata { get; set; } = [];
        }

        private class PaymentMethodResponse
        {
            public string Id { get; set; } = string.Empty;
            public string? Last4Digits { get; set; }
            public string? Brand { get; set; }
            public string? ExpiryMonth { get; set; }
            public string? ExpiryYear { get; set; }
        }

        private class PaymentMethodListResponse
        {
            public List<PaymentMethodResponse> Data { get; set; } = [];
        }

        private class PaymentError
        {
            public string? Message { get; set; }
            public string? Code { get; set; }
            public string? Type { get; set; }
        }
    }
}
