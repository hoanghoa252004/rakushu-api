using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingRoleLineRepository
	: BaseRepository<ShadowingRoleLine, ShadowingRoleLineId>, IShadowingRoleLineRepository
{
	public ShadowingRoleLineRepository(RakushuDbContext context) : base(context) { }

	public async Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleLineId? excludeId,
		CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingRoleLines.AnyAsync(
			l => l.ShadowingMaterialId == materialId
				&& l.SortOrder == sortOrder
				&& (excludeId == null || l.Id != excludeId),
			cancellationToken);
	}

	public async Task<int> GetNextSortOrderAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default)
	{
		var max = await _context.ShadowingRoleLines
			.Where(l => l.ShadowingMaterialId == materialId)
			.Select(l => (int?)l.SortOrder)
			.MaxAsync(cancellationToken);

		return (max ?? 0) + 1;
	}

	public async Task<(IReadOnlyList<ShadowingRoleLine> Items, int TotalCount)> GetPagedAsync(
		ShadowingMaterialId materialId,
		SpeakerId? speakerId,
		ShadowingRoleId? roleId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingRoleLines
			.AsNoTracking()
			.Where(l => l.ShadowingMaterialId == materialId);

		if (speakerId is not null)
			query = query.Where(l => l.ShadowingRole.SpeakerId == speakerId);
		if (roleId is not null)
			query = query.Where(l => l.ShadowingRoleId == roleId);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderBy(l => l.SortOrder)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}
}
