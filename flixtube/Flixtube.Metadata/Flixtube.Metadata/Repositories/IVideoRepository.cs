using Flixtube.Metadata.Data;
using Flixtube.Metadata.Entities;

namespace Flixtube.Metadata.Repositories;

public interface IVideoRepository : IRepository<ApplicationDbContext, Video>
{
}