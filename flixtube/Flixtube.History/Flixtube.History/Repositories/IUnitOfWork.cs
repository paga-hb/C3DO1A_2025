namespace Flixtube.History.Repositories;

public interface IUnitOfWork : IDisposable
{
    IViewHistoryRepository ViewHistorys { get; }
    Task<int> CompleteAsync();
}