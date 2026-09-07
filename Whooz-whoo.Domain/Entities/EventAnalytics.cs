using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class EventAnalytics : BaseEntity
    {
        public Guid EventId { get; private set; }
        public int PageViews { get; private set; }
        public int UniqueVisitors { get; private set; }
        public int ClickThroughs { get; private set; }
        public int ConversionRate { get; private set; }
        public decimal AverageTimeOnPage { get; private set; }
        public List<DailyAnalytics> DailyStats { get; private set; }

        private EventAnalytics() { }

        public EventAnalytics(Guid eventId)
        {
            EventId = eventId;
            PageViews = 0;
            UniqueVisitors = 0;
            ClickThroughs = 0;
            ConversionRate = 0;
            AverageTimeOnPage = 0;
            DailyStats = new List<DailyAnalytics>();
            CreatedAt = DateTime.UtcNow;
        }

        public void RecordPageView(string visitorId)
        {
            PageViews++;
            if (!DailyStats.Any(d => d.Date == DateOnly.FromDateTime(DateTime.UtcNow)))
            {
                DailyStats.Add(new DailyAnalytics(DateOnly.FromDateTime(DateTime.UtcNow)));
            }
            var today = DailyStats.First(d => d.Date == DateOnly.FromDateTime(DateTime.UtcNow));
            today.AddVisitor(visitorId);
            UpdateTimestamp();
        }

        public void RecordClickThrough()
        {
            ClickThroughs++;
            UpdateTimestamp();
        }
    }
}
