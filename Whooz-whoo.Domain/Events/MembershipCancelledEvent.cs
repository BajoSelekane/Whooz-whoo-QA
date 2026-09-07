using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Events
{
    public class MembershipCancelledEvent : IDomainEvent
    {
        public Membership Membership { get; }
        public DateTime OccurredOn { get; }

        public MembershipCancelledEvent(Membership membership)
        {
            Membership = membership;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
