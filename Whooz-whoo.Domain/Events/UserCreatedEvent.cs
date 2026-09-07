using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Events
{
    public class UserCreatedEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; }

        public UserCreatedEvent(User user)
        {
            User = user;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
