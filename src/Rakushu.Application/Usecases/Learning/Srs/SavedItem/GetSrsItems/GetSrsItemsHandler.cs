using System.Globalization;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

internal sealed class GetSrsItemsHandler : IRequestHandler<GetSrsItemsQuery, Result<PaginatedList<SavedItemSummaryDto>>>
{
	private const int MasteredIntervalDays = 21;

	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ILearnerEncounterEventRepository _encounterEventRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsItemsHandler(
		ISavedItemRepository savedItemRepository,
		ILearnerEncounterEventRepository encounterEventRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_encounterEventRepository = encounterEventRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<PaginatedList<SavedItemSummaryDto>>> Handle(
		GetSrsItemsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<PaginatedList<SavedItemSummaryDto>>(UserErrors.NotFound);
		}

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 500);

		var utcNow = _systemClock.UtcNow;
		var timeZone = AnalyticsTimeZoneHelper.ResolveTimeZone(request.TimeZone);
		var localToday = AnalyticsTimeZoneHelper.GetLocalToday(utcNow, timeZone);
		var (_, todayEndUtc) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(localToday, timeZone);

		DateTimeOffset? dueFromUtc = null;
		DateTimeOffset? dueBeforeUtc = null;
		if (!string.IsNullOrWhiteSpace(request.DueDate)
			&& DateOnly.TryParseExact(request.DueDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var targetDueDate))
		{
			if (targetDueDate <= localToday)
			{
				dueBeforeUtc = todayEndUtc;
			}
			else
			{
				(dueFromUtc, dueBeforeUtc) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(targetDueDate, timeZone);
			}
		}

		var options = new SavedItemQueryOptions(
			Page: page,
			PageSize: pageSize,
			Filter: request.Filter,
			SortBy: request.SortBy,
			SortOrder: request.SortOrder,
			Level: request.Level,
			KnowledgeType: request.KnowledgeType,
			Status: request.Status,
			Search: request.Search,
			IsActive: request.IsActive,
			DueFromUtc: dueFromUtc,
			DueBeforeUtc: dueBeforeUtc,
			EndOfTodayUtc: todayEndUtc,
			NowUtc: utcNow);

		int totalCount = await _savedItemRepository.CountUserItemsAsync(
			userId,
			options,
			cancellationToken);

		if (totalCount == 0)
		{
			return Result.Success(PaginatedList<SavedItemSummaryDto>.Create(
				[],
				0,
				page,
				pageSize));
		}

		var items = await _savedItemRepository.GetUserItemsAsync(
			userId,
			options,
			cancellationToken);

		if (items.Count == 0)
		{
			return Result.Success(PaginatedList<SavedItemSummaryDto>.Create(
				[],
				totalCount,
				page,
				pageSize));
		}

		var meaningIds = items.Select(i => i.KnowledgeMeaningId).Distinct().ToList();
		var sevenDaysAgo = utcNow.AddDays(-SrsSchedule.ImmersionWindowDays);
		var encounterCounts = await _encounterEventRepository.GetEncounterCountsByMeaningsAsync(
			userId,
			meaningIds,
			sevenDaysAgo,
			cancellationToken);

		var dtos = items.Select(si =>
		{
			var latestDetail = si.Schedule?.Details
				.OrderByDescending(d => d.ReviewNumber)
				.FirstOrDefault();

			var recentEncounters = encounterCounts.GetValueOrDefault(si.KnowledgeMeaningId, 0);

			var jlptLevel = si.KnowledgeMeaning?.KnowledgeUnit?.ProficiencyLevel?.Code;
			var knowledgeType = si.KnowledgeMeaning?.KnowledgeUnit?.KnowledgeType.ToString();
			var status = ResolveMaturityStatus(si.IsActive, latestDetail?.IntervalDays);
			bool isMastered = (latestDetail?.IntervalDays >= MasteredIntervalDays)
				|| string.Equals(status, "mastered", StringComparison.OrdinalIgnoreCase);

			return new SavedItemSummaryDto(
				si.Id.Value,
				si.KnowledgeMeaningId.Value,
				si.Expression,
				si.Reading,
				si.Meaning,
				si.IsActive,
				si.Schedule?.DueAt,
				si.SavedAt,
				si.IsRevoked,
				latestDetail?.Lapses,
				latestDetail?.EasinessFactor,
				latestDetail?.IntervalDays,
				latestDetail?.Repetitions,
				recentEncounters,
				jlptLevel,
				knowledgeType,
				status,
				isMastered);
		}).ToList();

		return Result.Success(PaginatedList<SavedItemSummaryDto>.Create(
			dtos,
			totalCount,
			page,
			pageSize));
	}

	private static string ResolveMaturityStatus(bool isActive, int? latestIntervalDays)
	{
		if (!isActive)
		{
			return "inactive";
		}

		if (!latestIntervalDays.HasValue)
		{
			return "new";
		}

		if (latestIntervalDays.Value >= MasteredIntervalDays)
		{
			return "mastered";
		}

		if (latestIntervalDays.Value >= 7)
		{
			return "young";
		}

		return "learning";
	}
}
