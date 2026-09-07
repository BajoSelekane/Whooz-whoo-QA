using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
