namespace Whooz_whoo.Domain.Interfaces
{
    public interface IEventRepository
    {
        IEnumerable<object> GetQueryable();
    }
}