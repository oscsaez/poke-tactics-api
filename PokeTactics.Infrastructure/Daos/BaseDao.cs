using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos
{
    public abstract class BaseDao<TEntity> : IBaseDao<TEntity> where TEntity : Entity
    {
        private readonly DbSet<TEntity> DbSet;
        protected readonly PokeTacticsContext DbContext;

        protected BaseDao(PokeTacticsContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual async Task DeleteByIdAsync(int id)
        {
            await DbSet.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public virtual async Task DeleteByIdsAsync(IEnumerable<int> ids)
        {
            await DbSet.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
        }

        // Should be used only for testing
        public virtual async Task DeleteAllAsync()
        {
            await DbSet.ExecuteDeleteAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> LoadAllAsync()
        {
            return await Query().ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> LoadAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = Query();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async ValueTask<TEntity?> LoadByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Query().SingleOrDefaultAsync(predicate);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        protected virtual IQueryable<TEntity> Query()
        {
            return DbSet;
        }
    }
}