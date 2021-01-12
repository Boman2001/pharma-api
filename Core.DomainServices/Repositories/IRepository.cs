using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain;

namespace Core.DomainServices.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T> Get(int id);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter);
        IEnumerable<T> Get(IEnumerable<string> includeProperties);
        IEnumerable<T> Get(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter, IEnumerable<string> includeProperties);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter, IEnumerable<string> includeProperties,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        Task<T> Add(T model);
        Task<T> Update(T model);
        Task Delete(int id);
        Task Delete(T model);
        Task ForceDelete(int id);
        Task ForceDelete(T model);
        Task Save();
        void Detach(IEnumerable<T> entities);
        void Detach(T entity);
    }
}