using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Interfaces;

namespace Whooz_whoo.Infrastructure.Services
{
    
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(
            IServiceProvider serviceProvider,
            ILogger<DomainEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            try
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var method = handlerType.GetMethod("HandleAsync");
                    if (method != null)
                    {
                        await (Task)method.Invoke(handler, new object[] { domainEvent });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching domain event {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }
    }
}
