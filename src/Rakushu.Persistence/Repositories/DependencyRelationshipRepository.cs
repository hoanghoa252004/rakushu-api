using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Persistence.Repositories;

public sealed class DependencyRelationshipRepository : BaseRepository<DependencyRelationship, DependencyRelationshipId>, IDependencyRelationshipRepository
{
	public DependencyRelationshipRepository(RakushuDbContext context) : base(context) { }

	public async Task<DependencyRelationship?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.DependencyRelationships
			.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
	}
}
