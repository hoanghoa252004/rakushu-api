using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService
{
	private static readonly KnowledgeCollectionType[] OrderedCollectionTypes =
	[
		KnowledgeCollectionType.Word,
		KnowledgeCollectionType.Collocation,
		KnowledgeCollectionType.Grammar,
		KnowledgeCollectionType.Terminology,
		KnowledgeCollectionType.ProperNoun,
		KnowledgeCollectionType.Mixed
	];

	private async Task<CuratorKnowledgeAnalyticsDto> GetKnowledgeSectionAsync(
		DateTimeOffset windowStartUtc,
		int lowConfidenceLearningUnitsCount,
		CancellationToken cancellationToken)
	{
		// 1. Collection counts grouped by (CollectionType, IsActive).
		var collectionGroups = await _context.KnowledgeCollections
			.AsNoTracking()
			.GroupBy(c => new { c.CollectionType, c.IsActive })
			.Select(g => new
			{
				g.Key.CollectionType,
				g.Key.IsActive,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		int activeCollectionsCount = collectionGroups.Where(x => x.IsActive).Sum(x => x.Count);
		int inactiveCollectionsCount = collectionGroups.Where(x => !x.IsActive).Sum(x => x.Count);
		int totalCollections = activeCollectionsCount + inactiveCollectionsCount;

		var collectionTypeDistribution = OrderedCollectionTypes.Select(type =>
		{
			int total = collectionGroups.Where(x => x.CollectionType == type).Sum(x => x.Count);
			int active = collectionGroups.Where(x => x.CollectionType == type && x.IsActive).Sum(x => x.Count);
			return new CuratorCollectionTypeBreakdownDto(type.ToString(), total, active);
		}).ToList();

		// 2. Knowledge unit counts grouped by (SourceType, IsActive).
		var unitGroups = await _context.KnowledgeUnits
			.AsNoTracking()
			.GroupBy(u => new { u.SourceType, u.IsActive })
			.Select(g => new
			{
				g.Key.SourceType,
				g.Key.IsActive,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		int activeUnitsCount = unitGroups.Where(x => x.IsActive).Sum(x => x.Count);
		int pendingUnitsCount = unitGroups.Where(x => !x.IsActive).Sum(x => x.Count);
		int totalKnowledgeUnits = activeUnitsCount + pendingUnitsCount;
		int oovDetectedUnitsCount = unitGroups
			.Where(x => x.SourceType == KnowledgeUnitSourceType.OovDetected)
			.Sum(x => x.Count);
		int pendingOovUnitsCount = unitGroups
			.Where(x => x.SourceType == KnowledgeUnitSourceType.OovDetected && !x.IsActive)
			.Sum(x => x.Count);

		int periodNewUnitsCount = await _context.KnowledgeUnits
			.AsNoTracking()
			.CountAsync(u => u.CreatedAt >= windowStartUtc, cancellationToken);

		// 3. Featured collections with exact unit counts (single GROUP BY + metadata fetch).
		var unitCountsByCollectionRaw = await _context.KnowledgeUnits
			.AsNoTracking()
			.GroupBy(u => u.KnowledgeCollectionId)
			.Select(g => new
			{
				CollectionId = g.Key,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		var unitCountsByCollection = unitCountsByCollectionRaw.ToDictionary(
			x => x.CollectionId.Value,
			x => x.Count);

		var allCollectionsRaw = await _context.KnowledgeCollections
			.AsNoTracking()
			.Select(c => new
			{
				c.Id,
				c.Code,
				c.Name,
				c.CollectionType,
				c.Scope,
				c.IsActive,
				c.UpdatedAt
			})
			.ToListAsync(cancellationToken);

		var featuredCollections = allCollectionsRaw
			.Select(c => new CuratorCollectionSummaryItemDto(
				c.Id.Value,
				c.Code,
				c.Name,
				c.CollectionType.ToString(),
				c.Scope.ToString(),
				c.IsActive,
				unitCountsByCollection.GetValueOrDefault(c.Id.Value),
				c.UpdatedAt))
			.OrderByDescending(c => c.IsActive)
			.ThenByDescending(c => c.ItemsCount)
			.ThenByDescending(c => c.UpdatedAt)
			.Take(PriorityQueueLimit)
			.ToList();

		return new CuratorKnowledgeAnalyticsDto(
			totalCollections,
			activeCollectionsCount,
			inactiveCollectionsCount,
			totalKnowledgeUnits,
			activeUnitsCount,
			pendingUnitsCount,
			oovDetectedUnitsCount,
			pendingOovUnitsCount,
			lowConfidenceLearningUnitsCount,
			periodNewUnitsCount,
			collectionTypeDistribution,
			featuredCollections);
	}
}
