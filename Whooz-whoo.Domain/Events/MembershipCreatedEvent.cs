using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Events
{
    public class MembershipCreatedEvent : IDomainEvent
    {
        public Membership Membership { get; }
        public DateTime OccurredOn { get; }

        public MembershipCreatedEvent(Membership membership)
        {
            Membership = membership;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
