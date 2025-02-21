using Flixtube.Data;

namespace Flixtube.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public IVideoRepository Videos { get; private set; }
    public IViewHistoryRepository ViewHistorys { get; private set; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        Videos = new VideoRepository(_dbContext);
        ViewHistorys = new ViewHistoryRepository(_dbContext);
    }

    public async Task<int> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}