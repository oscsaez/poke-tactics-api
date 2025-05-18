using System.Linq.Expressions;
using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos
{
    public interface IBaseDao<TEntity> where TEntity : Entity
    {
        ValueTask<TEntity> LoadByIdAsync(int id);

        Task<IEnumerable<TEntity>> LoadAllAsync();

        Task<IEnumerable<TEntity>> LoadAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        Task CreateAsync(TEntity entity);

        Task CreateRangeAsync(IEnumerable<TEntity> entities);

        Task DeleteByIdAsync(int id);

        Task DeleteByIdsAsync(IEnumerable<int> ids);

        Task UpdateAsync(TEntity entity);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    }
}