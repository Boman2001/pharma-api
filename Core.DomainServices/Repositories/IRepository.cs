using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain.Models;

namespace Core.DomainServices.Repositories
{
    using Microsoft.AspNetCore.Identity;

    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> Get();
        Task<T> Get(int id);
        T Get(int id, IEnumerable<string> includeProperties);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter);
        IEnumerable<T> Get(IEnumerable<string> includeProperties);
        IEnumerable<T> Get(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter, IEnumerable<string> includeProperties);

        IEnumerable<T> Get(Expression<Func<T, bool>> filter, IEnumerable<string> includeProperties,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        Task<T> Add(T model, IdentityUser identityUser);
        Task<T> Update(T model, IdentityUser identityUser);
        Task Delete(int id, IdentityUser identityUser);
        Task Delete(T model, IdentityUser identityUser);
        Task Save();
        void Detach(IEnumerable<T> entities);
        void Detach(T entity);
    }
}