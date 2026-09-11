using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Application.DTOs
{
    public class MembershipDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public MembershipType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
        public int TrialDaysRemaining { get; set; }
        public decimal Price { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<MembershipBenefitDto> Benefits { get; set; }
        public decimal AnnualROI { get; set; }
        public int EventsAttended { get; set; }
        public int TotalSavings { get; set; }
    }
}
