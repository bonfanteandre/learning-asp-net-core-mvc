using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll(string[]? includes = null)
        {
            var query = _dbSet.AsQueryable();

            if (includes != null && includes.Length > 0)
            {
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string[]? includes = null)
        {
            var query = _dbSet.Where(filter);

            if (includes != null && includes.Length > 0)
            {
                foreach (string include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.FirstOrDefault();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
