using Flixtube.Data;
using Flixtube.Data.Entities;

namespace Flixtube.Data.Repositories;

public class ViewHistoryRepository : Repository<ApplicationDbContext, ViewHistory>, IViewHistoryRepository
{
    public ViewHistoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}