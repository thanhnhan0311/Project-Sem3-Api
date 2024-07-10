using arts_core.Data;
using System.Linq.Expressions;

namespace arts_core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);        
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void SaveChanges();        
    }

    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected DataContext _context;
        protected GenericRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }       

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges(); 
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }
    }
}
