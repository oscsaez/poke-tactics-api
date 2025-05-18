using PokeTactics.Core.Interfaces;

namespace PokeTactics.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PokeTacticsContext _dbContext;
        // Set here private readonly DAOs

        public UnitOfWork(PokeTacticsContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Set lazy loading for DAOs (instance DAOs only if they are used)

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
        }
    }
}