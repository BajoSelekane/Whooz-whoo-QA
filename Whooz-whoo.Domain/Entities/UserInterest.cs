namespace Whooz_whoo.Domain.Entities
{
    public class UserInterest : ValueObject
    {
        public string Category { get; private set; }
        public int InterestLevel { get; private set; } // 1-10

        public UserInterest(string category, int interestLevel)
        {
            Category = category;
            InterestLevel = Math.Clamp(interestLevel, 1, 10);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Category;
            yield return InterestLevel;
        }
    }
}