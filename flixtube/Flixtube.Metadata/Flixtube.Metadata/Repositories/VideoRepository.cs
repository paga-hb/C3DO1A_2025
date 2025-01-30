using Flixtube.Metadata.Data;
using Flixtube.Metadata.Entities;

namespace Flixtube.Metadata.Repositories;

public class VideoRepository : Repository<ApplicationDbContext, Video>, IVideoRepository
{
    public VideoRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}