using Flixtube.History.Data;
using Flixtube.History.Entities;

namespace Flixtube.History.Repositories;

public interface IViewHistoryRepository : IRepository<ApplicationDbContext, ViewHistory>
{
}