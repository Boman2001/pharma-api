using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;

    public class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly DbSet<Activity> _activities;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _activities = _context.Set<Activity>();
        }

        public IEnumerable<T> Get()
        {
            return _dbSet.ToList();
        }

        public async Task<T> Get(int id)
        {
            return await _dbSet.SingleOrDefaultAsync(e => e.Id == id);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _dbSet;

            if (filter == null) return query.ToList();

            query = query.Where(filter);

            return query.ToList();
        }

        public IEnumerable<T> Get(IEnumerable<string> includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        public IEnumerable<T> Get(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            IQueryable<T> query = _dbSet;

            return orderBy(query).ToList();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter, IEnumerable<string> includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (filter == null) return query.ToList();

            query = query.Where(filter);

            return query.ToList();
        }

        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter,
            IEnumerable<string> includeProperties,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        public async Task<T> Add(T entity, IdentityUser identityUser)
        {
            Guid guid;

            try
            {
                guid = Guid.Parse(identityUser.Id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid credentials");
            }

            entity.CreatedBy = guid;

            await _dbSet.AddAsync(entity);
            await _context.Entry(entity).GetDatabaseValuesAsync();

            await _activities.AddAsync(new Activity
            {
                Description = "Add",
                Properties = JsonConvert.SerializeObject(entity),
                SubjectId = entity.Id,
                SubjectType = typeof(T).ToString(),
                CreatedBy = guid
            });

            await Save();

            return entity;
        }

        public async Task<T> Update(T entity, IdentityUser identityUser)
        {
            Guid guid;

            try
            {
                guid = Guid.Parse(identityUser.Id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid credentials.");
            }

            entity.UpdatedBy = guid;
            entity.UpdatedAt = DateTime.Now;

            _dbSet.Update(entity);
            await Save();

            await _activities.AddAsync(new Activity
            {
                Description = "Update",
                Properties = JsonConvert.SerializeObject(entity),
                SubjectId = entity.Id,
                SubjectType = typeof(T).ToString(),
                CreatedBy = guid
            });

            await Save();

            return entity;
        }

        public async Task Delete(int id, IdentityUser identityUser)
        {
            var entity = await Get(id);

            if (entity == null)
            {
                return;
            }

            Guid guid;

            try
            {
                guid = Guid.Parse(identityUser.Id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid credentials.");
            }

            entity.DeletedBy = guid;
            entity.DeletedAt = DateTime.Now;

            _dbSet.Remove(entity);
            await Save();

            await _activities.AddAsync(new Activity
            {
                Description = "Delete",
                Properties = JsonConvert.SerializeObject(entity),
                SubjectId = entity.Id,
                SubjectType = typeof(T).ToString(),
                CreatedBy = guid
            });
            
            await Save();
        }

        public async Task Delete(T entity, IdentityUser identityUser)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Guid guid;

            try
            {
                guid = Guid.Parse(identityUser.Id);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid credentials.");
            }

            _dbSet.Remove(entity);
            await Save();

            await _activities.AddAsync(new Activity
            {
                Description = "Delete",
                Properties = JsonConvert.SerializeObject(entity),
                SubjectId = entity.Id,
                SubjectType = typeof(T).ToString(),
                DeletedBy = guid,
                DeletedAt = DateTime.Now,
            });
            
            await Save();
        }

        public async Task ForceDelete(int id)
        {
            var entity = await Get(id);

            if (entity == null)
            {
                return;
            }

            _dbSet.Remove(entity);
            await Save();
        }

        public async Task ForceDelete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbSet.Remove(entity);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Detach(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
        }

        public void Detach(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}