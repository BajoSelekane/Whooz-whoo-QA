using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Events
{
    public class UserVerifiedEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; }

        public UserVerifiedEvent(User user)
        {
            User = user;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
