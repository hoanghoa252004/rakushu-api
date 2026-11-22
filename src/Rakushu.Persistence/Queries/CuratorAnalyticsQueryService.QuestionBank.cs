using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService
{
	private async Task<CuratorQuestionBankAnalyticsDto> GetQuestionBankSectionAsync(
		DateTimeOffset windowStartUtc,
		CancellationToken cancellationToken)
	{
		// 1. All canonical question types so zero-count types still render deterministically.
		var questionTypes = await _context.QuestionTypes
			.AsNoTracking()
			.OrderBy(qt => qt.Code)
			.Select(qt => new
			{
				qt.Id,
				qt.Code,
				qt.Name
			})
			.ToListAsync(cancellationToken);

		// 2. Single aggregate GROUP BY across (QuestionTypeId, Status, IsActive, MediaType).
		var questionGroups = await _context.BankQuestions
			.AsNoTracking()
			.GroupBy(q => new
			{
				q.QuestionTypeId,
				q.Status,
				q.IsActive,
				q.MediaType
			})
			.Select(g => new
			{
				g.Key.QuestionTypeId,
				g.Key.Status,
				g.Key.IsActive,
				g.Key.MediaType,
				Count = g.Count()
			})
			.ToListAsync(cancellationToken);

		int publishedCount = 0;
		int draftCount = 0;
		int inactiveCount = 0;
		int withAudioCount = 0;
		int withVideoCount = 0;
		int withImageCount = 0;
		int textOnlyCount = 0;

		var publishedByType = new Dictionary<Guid, int>();
		var draftByType = new Dictionary<Guid, int>();

		foreach (var group in questionGroups)
		{
			if (group.Status == QuestionStatus.Published && !group.IsActive)
			{
				inactiveCount += group.Count;
				continue;
			}

			var typeId = group.QuestionTypeId.Value;

			if (group.Status == QuestionStatus.Draft)
			{
				draftCount += group.Count;
				draftByType[typeId] = draftByType.GetValueOrDefault(typeId) + group.Count;
			}
			else if (group.Status == QuestionStatus.Published && group.IsActive)
			{
				publishedCount += group.Count;
				publishedByType[typeId] = publishedByType.GetValueOrDefault(typeId) + group.Count;
			}

			var mediaType = (group.MediaType ?? "NONE").Trim().ToUpperInvariant();
			switch (mediaType)
			{
				case "AUDIO":
					withAudioCount += group.Count;
					break;
				case "VIDEO":
					withVideoCount += group.Count;
					break;
				case "IMAGE":
					withImageCount += group.Count;
					break;
				default:
					textOnlyCount += group.Count;
					break;
			}
		}

		// 3. Period activity counts (valid non-soft-deleted questions).
		int periodCreatedCount = await _context.BankQuestions
			.AsNoTracking()
			.CountAsync(
				q => (q.Status == QuestionStatus.Draft || q.IsActive) && q.CreatedAt >= windowStartUtc,
				cancellationToken);

		int periodReviewedCount = await _context.BankQuestions
			.AsNoTracking()
			.CountAsync(
				q => q.Status == QuestionStatus.Published && q.IsActive && q.ReviewedAt != null && q.ReviewedAt >= windowStartUtc,
				cancellationToken);

		var typeDistribution = questionTypes.Select(qt =>
		{
			int pub = publishedByType.GetValueOrDefault(qt.Id.Value);
			int dr = draftByType.GetValueOrDefault(qt.Id.Value);
			return new CuratorQuestionTypeBreakdownDto(
				qt.Id.Value,
				qt.Code,
				qt.Name,
				pub + dr,
				pub,
				dr);
		}).OrderByDescending(x => x.TotalCount).ThenBy(x => x.Code).ToList();

		// 4. Recent draft questions queue (max 6, newest first).
		var recentDraftRaw = await _context.BankQuestions
			.AsNoTracking()
			.Where(q => q.Status == QuestionStatus.Draft)
			.OrderByDescending(q => q.CreatedAt)
			.Take(PriorityQueueLimit)
			.Select(q => new
			{
				q.Id,
				QuestionTypeCode = q.QuestionType.Code,
				QuestionTypeName = q.QuestionType.Name,
				q.Question,
				KnowledgeUnitExpression = q.KnowledgeMeaning != null && q.KnowledgeMeaning.KnowledgeUnit != null
					? q.KnowledgeMeaning.KnowledgeUnit.Expression
					: null,
				q.MediaType,
				q.CreatedAt
			})
			.ToListAsync(cancellationToken);

		var recentDraftQuestions = recentDraftRaw.Select(q => new CuratorDraftQuestionItemDto(
			q.Id.Value,
			q.QuestionTypeCode,
			q.QuestionTypeName,
			TruncateText(q.Question, 140),
			q.KnowledgeUnitExpression,
			string.IsNullOrWhiteSpace(q.MediaType) ? "NONE" : q.MediaType.Trim().ToUpperInvariant(),
			q.CreatedAt)).ToList();

		return new CuratorQuestionBankAnalyticsDto(
			publishedCount + draftCount,
			publishedCount,
			draftCount,
			inactiveCount,
			withAudioCount + withVideoCount + withImageCount,
			withAudioCount,
			withVideoCount,
			withImageCount,
			textOnlyCount,
			periodCreatedCount,
			periodReviewedCount,
			typeDistribution,
			recentDraftQuestions);
	}
}
