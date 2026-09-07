namespace Whooz_whoo.Domain.Entities
{
    public class EventTag : ValueObject
    {
        public string Name { get; private set; }
        public string? Color { get; private set; }

        public EventTag(string name, string? color = null)
        {
            Name = name;
            Color = color;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}