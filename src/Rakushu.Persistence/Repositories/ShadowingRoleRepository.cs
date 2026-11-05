using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingRoleRepository
	: BaseRepository<ShadowingRole, ShadowingRoleId>, IShadowingRoleRepository
{
	public ShadowingRoleRepository(RakushuDbContext context) : base(context) { }

	public async Task<IReadOnlyList<ShadowingRole>> GetByMaterialIdAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoles
			.AsNoTracking()
			.Include(r => r.Speaker)
			.Where(r => r.ShadowingMaterialId == materialId)
			.OrderBy(r => r.SortOrder)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> SpeakerExistsAsync(
		ShadowingMaterialId materialId,
		SpeakerId speakerId,
		ShadowingRoleId? excludeId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoles.AnyAsync(
			r => r.ShadowingMaterialId == materialId
				&& r.SpeakerId == speakerId
				&& (excludeId == null || r.Id != excludeId),
			cancellationToken);
	}

	public async Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleId? excludeId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoles.AnyAsync(
			r => r.ShadowingMaterialId == materialId
				&& r.SortOrder == sortOrder
				&& (excludeId == null || r.Id != excludeId),
			cancellationToken);
	}
}
