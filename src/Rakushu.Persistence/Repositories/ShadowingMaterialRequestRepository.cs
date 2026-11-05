using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Persistence.Repositories;

public sealed class ShadowingMaterialRequestRepository
	: BaseRepository<ShadowingMaterialRequest, ShadowingMaterialRequestId>, IShadowingMaterialRequestRepository
{
	public ShadowingMaterialRequestRepository(RakushuDbContext context) : base(context) { }

	public async Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default)
	{
		var codes = await _context.ShadowingMaterialRequests
			.AsNoTracking()
			.Select(r => r.Code)
			.ToListAsync(cancellationToken);

		var max = codes
			.Select(c => c.StartsWith("SMR-") && int.TryParse(c.AsSpan(4), out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return $"SMR-{max + 1:D4}";
	}

	public async Task<bool> ExistsByVideoIdAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		return await _context.ShadowingMaterialRequests
			.AnyAsync(r => r.VideoId == videoId && r.Status != ShadowingMaterialRequestStatus.Rejected, cancellationToken);
	}

	public async Task<(IReadOnlyList<ShadowingMaterialRequest> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		VideoId? videoId,
		UserId? learnerId,
		ShadowingMaterialRequestStatus? status,
		CancellationToken cancellationToken = default)
	{
		var query = _context.ShadowingMaterialRequests.AsNoTracking().AsQueryable();

		if (videoId is not null)
			query = query.Where(r => r.VideoId == videoId);
		if (learnerId is not null)
			query = query.Where(r => r.LearnerId == learnerId);
		if (status is not null)
			query = query.Where(r => r.Status == status);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(r => r.RequestedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}
}
