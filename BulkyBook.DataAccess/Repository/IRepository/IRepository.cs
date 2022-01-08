using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string[]? includes = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string[]? includes = null);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
