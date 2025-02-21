using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Flixtube.Data.Repositories;

public interface IRepository<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    Task<IEnumerable<TEntity>> FindAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}