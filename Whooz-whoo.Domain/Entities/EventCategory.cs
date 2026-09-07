using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class EventCategory : ValueObject
    {
        public EventType Type { get; private set; }
        public string SubCategory { get; private set; }

        public EventCategory(EventType type, string subCategory = "")
        {
            Type = type;
            SubCategory = subCategory;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return SubCategory;
        }
    }
}