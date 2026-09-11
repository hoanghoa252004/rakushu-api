namespace Rakushu.Domain.Common.Contract;

public interface IUnitOfWork
{
	Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
