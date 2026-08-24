using Rakushu.Domain.Common.Contract;
using Rakushu.Persistence.DbContext;

namespace Rakushu.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
	private readonly RakushuDbContext _context;

	public UnitOfWork(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await _context.SaveChangesAsync(cancellationToken);
	}
}
