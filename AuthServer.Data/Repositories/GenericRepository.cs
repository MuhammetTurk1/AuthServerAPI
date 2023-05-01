using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _Context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _Context = context;
            _dbSet= context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entitiy)
        {
            await _dbSet.AddAsync(entitiy);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
           var entity= await _dbSet.FindAsync(id);
            if (entity !=null)
            {
                _Context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public void Remove(TEntity entitiy)
        {
            _dbSet.Remove(entitiy);
        }

        public TEntity Update(TEntity entitiy)
        {
            _Context.Entry(entitiy).State = EntityState.Modified;
            return entitiy;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
