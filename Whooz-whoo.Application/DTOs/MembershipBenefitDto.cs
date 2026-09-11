namespace Whooz_whoo.Application.DTOs
{
    public class MembershipBenefitDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
        public int CurrentUsage { get; set; }
        public int RemainingUses => UsageLimit > 0 ? UsageLimit - CurrentUsage : int.MaxValue;
        public bool IsAvailable => IsActive && (UsageLimit == 0 || CurrentUsage < UsageLimit);
        public string Icon { get; set; }
        public string Color { get; set; }
        public BenefitCategory Category { get; set; }
        public string HowToUse { get; set; }
        public List<BenefitRestriction> Restrictions { get; set; }
    }

}