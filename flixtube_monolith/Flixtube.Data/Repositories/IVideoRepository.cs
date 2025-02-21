using Flixtube.Data;
using Flixtube.Data.Entities;

namespace Flixtube.Data.Repositories;

public interface IVideoRepository : IRepository<ApplicationDbContext, Video>
{
}