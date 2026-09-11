using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Events;
using Whooz_whoo.Domain.Interfaces;

namespace Whooz_whoo.Application.Common.Handlers
{
    public class MembershipCreatedEventHandler : IDomainEventHandler<MembershipCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MembershipCreatedEventHandler> _logger;

        public MembershipCreatedEventHandler(
            INotificationService notificationService,
            ILogger<MembershipCreatedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(MembershipCreatedEvent domainEvent)
        {
            try
            {
                var membership = domainEvent.Membership;
                _logger.LogInformation("Membership created for user {UserId}", membership.UserId);

                // Send welcome email
                await _notificationService.SendEmailAsync(
                    "user@example.com", // Get from user repository
                    "Welcome to Gauteng Digital Hub!",
                    $"Your membership has been created. Start date: {membership.StartDate}"
                );

                // Send SMS notification
                await _notificationService.SendSmsAsync(
                    "0123456789", // Get from user repository
                    "Welcome to Gauteng Digital Hub! Your membership is active."
                );

                // Add to analytics
                // Track membership creation for ROI calculation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MembershipCreatedEvent for user {UserId}", domainEvent.Membership.UserId);
                throw;
            }
        }

        public Task HandleAsync(MembershipCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;
        private readonly ICacheService _cacheService;

        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task HandleAsync(UserCreatedEvent domainEvent)
        {
            try
            {
                var user = domainEvent.User;
                _logger.LogInformation("New user created: {UserId}", user.Id);

                // Cache user data for quick access
                await _cacheService.SetAsync($"user:{user.Id}", user, TimeSpan.FromHours(24));

                // Initialize user analytics
                // Create default settings
                // Send welcome notification
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserCreatedEvent for user {UserId}", domainEvent.User.Id);
                throw;
            }
        }

        public Task HandleAsync(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
