using System.Linq.Expressions;

namespace Rakushu.Domain.Common.Contract;

public interface IBaseRepository<TEntity, in TKey> where TEntity : class
{
	Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

	Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<IEnumerable<TEntity>> FindAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken = default);

	void Add(TEntity entity);

	void Update(TEntity entity);

	void Delete(TEntity entity);

	void DeleteMultiple(List<TEntity> entities);
}