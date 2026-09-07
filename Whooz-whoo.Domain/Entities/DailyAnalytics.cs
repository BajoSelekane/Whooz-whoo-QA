namespace Whooz_whoo.Domain.Entities
{
    public class DailyAnalytics : ValueObject
    {
        public DateOnly Date { get; private set; }
        public int DailyViews { get; private set; }
        public HashSet<string> UniqueVisitors { get; private set; }
        public int DailyClicks { get; private set; }
        public int DailyConversions { get; private set; }

        public DailyAnalytics(DateOnly date)
        {
            Date = date;
            DailyViews = 0;
            UniqueVisitors = new HashSet<string>();
            DailyClicks = 0;
            DailyConversions = 0;
        }

        public void AddVisitor(string visitorId)
        {
            UniqueVisitors.Add(visitorId);
            DailyViews++;
        }

        public void AddClick()
        {
            DailyClicks++;
        }

        public void AddConversion()
        {
            DailyConversions++;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Date;
        }
    }
}