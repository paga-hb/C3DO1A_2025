using Flixtube.History.Data;
using Flixtube.History.Entities;

namespace Flixtube.History.Repositories;

public class ViewHistoryRepository : Repository<ApplicationDbContext, ViewHistory>, IViewHistoryRepository
{
    public ViewHistoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}