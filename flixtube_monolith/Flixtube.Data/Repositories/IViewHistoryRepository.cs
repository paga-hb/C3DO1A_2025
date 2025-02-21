using Flixtube.Data;
using Flixtube.Data.Entities;

namespace Flixtube.Data.Repositories;

public interface IViewHistoryRepository : IRepository<ApplicationDbContext, ViewHistory>
{
}