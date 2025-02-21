namespace Flixtube.Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    IVideoRepository Videos { get; }
    IViewHistoryRepository ViewHistorys { get; }
    Task<int> CompleteAsync();
}