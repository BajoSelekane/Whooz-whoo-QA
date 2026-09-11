namespace Whooz_whoo.Application.DTOs
{
    public class BenefitRestriction
    {
        public string Type { get; set; } // "limit_per_day", "limit_per_month", "min_spend", etc.
        public string Value { get; set; }
        public string Description { get; set; }
    }
}