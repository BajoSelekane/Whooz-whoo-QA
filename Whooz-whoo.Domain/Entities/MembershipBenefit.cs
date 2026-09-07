using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class MembershipBenefit : ValueObject
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public int UsageLimit { get; private set; }
        public int CurrentUsage { get; private set; }

        public MembershipBenefit(string code, string name, string description, int usageLimit = 0)
        {
            Code = code;
            Name = name;
            Description = description;
            IsActive = true;
            UsageLimit = usageLimit;
            CurrentUsage = 0;
        }

        public bool CanUse()
        {
            if (!IsActive) return false;
            if (UsageLimit == 0) return true;
            return CurrentUsage < UsageLimit;
        }

        public void Use()
        {
            if (!CanUse())
                throw new BusinessException($"Benefit {Name} cannot be used");

            CurrentUsage++;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}
