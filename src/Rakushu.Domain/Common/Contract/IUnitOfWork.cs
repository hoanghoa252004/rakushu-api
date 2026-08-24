namespace Rakushu.Domain.Common.Contract;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
