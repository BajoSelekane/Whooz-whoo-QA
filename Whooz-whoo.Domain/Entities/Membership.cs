using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Enums;
using Whooz_whoo.Domain.Events;

namespace Whooz_whoo.Domain.Entities
{
    public class Membership : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public MembershipType Type { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsTrial { get; private set; }
        public int TrialDaysRemaining { get; private set; }
        public decimal Price { get; private set; }
        public string StripeSubscriptionId { get; private set; }
        public string TransactionId { get; private set; }
        public DateTime? LastPaymentDate { get; private set; }
        public DateTime? NextPaymentDate { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public List<MembershipBenefit> Benefits { get; private set; }
        public List<PaymentHistory> PaymentHistory { get; private set; }

        private Membership() { }

        public Membership(
            Guid userId,
            MembershipType type,
            DateTime startDate,
            DateTime endDate,
            string transactionId,
            bool isTrial = false,
            int trialDays = 0)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
            IsTrial = isTrial;
            TrialDaysRemaining = trialDays;
            TransactionId = transactionId;
            Price = GetPriceForMembershipType(type);
            PaymentStatus = PaymentStatus.Completed;
            Benefits = [];
            PaymentHistory = [];
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new MembershipCreatedEvent(this));
        }

        private static decimal GetPriceForMembershipType(MembershipType type) => type switch
        {
            MembershipType.Free => 0,
            MembershipType.Premium => 299.99m,
            MembershipType.PremiumPlus => 499.99m,
            MembershipType.Organizer => 999.99m,
            MembershipType.Enterprise => 1999.99m,
            _ => 0
        };

        public void Renew(DateTime newEndDate, string transactionId, decimal price)
        {
            EndDate = newEndDate;
            IsActive = true;
            TransactionId = transactionId;
            Price = price;
            LastPaymentDate = DateTime.UtcNow;
            NextPaymentDate = newEndDate;
            PaymentStatus = PaymentStatus.Completed;
            UpdateTimestamp();

            PaymentHistory.Add(new PaymentHistory(
                transactionId,
                price,
                PaymentStatus.Completed,
                DateTime.UtcNow
            ));

            AddDomainEvent(new MembershipRenewedEvent(this));
        }

        public void Cancel()
        {
            IsActive = false;
            PaymentStatus = PaymentStatus.Cancelled;
            UpdateTimestamp();

            AddDomainEvent(new MembershipCancelledEvent(this));
        }

        public void UpdateBenefits(List<MembershipBenefit> benefits)
        {
            Benefits = benefits;
            UpdateTimestamp();
        }

        public bool HasBenefit(string benefitCode)
        {
            return Benefits.Any(b => b.Code == benefitCode && b.IsActive);
        }

        public decimal CalculateAnnualROI()
        {
            // Simplified ROI calculation based on member engagement
            var baseValue = Type switch
            {
                MembershipType.Free => 50,
                MembershipType.Premium => 1200,
                MembershipType.PremiumPlus => 2400,
                MembershipType.Organizer => 5000,
                MembershipType.Enterprise => 10000,
                _ => 0
            };

            // Add engagement multiplier
            var engagementMultiplier = 1 + (PaymentHistory.Count * 0.1);
            return (decimal)(baseValue * engagementMultiplier);
        }
    }
}
