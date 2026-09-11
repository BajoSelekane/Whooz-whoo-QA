using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;

namespace Whooz_whoo.Domain.Interfaces
{
    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
