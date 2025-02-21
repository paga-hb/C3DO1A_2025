using Flixtube.Data;
using Flixtube.Data.Entities;

namespace Flixtube.Data.Repositories;

public class VideoRepository : Repository<ApplicationDbContext, Video>, IVideoRepository
{
    public VideoRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}