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
        protected readonly PokeTacticsContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

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
            TEntity entity = await LoadByIdAsync(id) ?? throw new EntityDoesNotExistException($"Entity with Id [{id}] does not exist");
            DbSet.Remove(entity);
        }

        public virtual async Task DeleteByIdsAsync(IEnumerable<int> ids)
        {
            IEnumerable<TEntity> entities = await LoadAsync(x => ids.Contains(x.Id));
            DbSet.RemoveRange(entities);
        }

        public virtual async Task<IEnumerable<TEntity>> LoadAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> LoadAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = DbSet;

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
            return await DbSet.SingleOrDefaultAsync(predicate);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;

            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.AttachRange(entities);
            DbContext.Entry(entities).State = EntityState.Modified;

            return Task.CompletedTask;
        }
    }
}