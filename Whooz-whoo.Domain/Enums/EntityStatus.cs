using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Enums
{
    public enum EntityStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Active = 2,
        Cancelled = 3,
        Completed = 4,
        SoldOut = 5,
        Postponed = 6,
        Inactive = 7
    }
}
