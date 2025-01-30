namespace Flixtube.Metadata.Repositories;

public interface IUnitOfWork : IDisposable
{
    IVideoRepository Videos { get; }
    Task<int> CompleteAsync();
}