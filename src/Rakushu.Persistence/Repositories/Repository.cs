using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Common.Contract;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.Repositories;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
	where TEntity : class
{
	protected readonly RakushuDbContext _context;
	protected readonly DbSet<TEntity> _dbSet;

	public Repository(RakushuDbContext context)
	{
		_context = context;
		_dbSet = context.Set<TEntity>();
	}

	public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
	{
		return await _dbSet.FindAsync([id], cancellationToken);
	}

	public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet.ToListAsync(cancellationToken);
	}

	public virtual async Task<IEnumerable<TEntity>> FindAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken = default)
	{
		return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
	}

	public virtual void Add(TEntity entity)
	{
		_dbSet.Add(entity);
	}

	public virtual void Update(TEntity entity)
	{
		_dbSet.Update(entity);
	}

	public virtual void Delete(TEntity entity)
	{
		_dbSet.Remove(entity);
	}

	public virtual void DeleteMultiple(List<TEntity> entities)
	{
		_dbSet.RemoveRange(entities);
	}
}
