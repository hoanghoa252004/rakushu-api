using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

internal sealed class GetSrsItemsHandler : IRequestHandler<GetSrsItemsQuery, Result<IReadOnlyList<SavedItemSummaryDto>>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsItemsHandler(
		ISavedItemRepository savedItemRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_encounterRepository = encounterRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<IReadOnlyList<SavedItemSummaryDto>>> Handle(
		GetSrsItemsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<IReadOnlyList<SavedItemSummaryDto>>(UserErrors.NotFound);

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var items = await _savedItemRepository.GetUserItemsAsync(
			userId,
			page,
			pageSize,
			request.Filter,
			request.SortBy,
			request.SortOrder,
			cancellationToken);

		if (items.Count == 0)
			return Result.Success<IReadOnlyList<SavedItemSummaryDto>>(Array.Empty<SavedItemSummaryDto>());

		var meaningIds = items.Select(i => i.KnowledgeMeaningId).Distinct().ToList();
		var sevenDaysAgo = _systemClock.UtcNow.AddDays(-7);
		var encounterCounts = await _encounterRepository.GetEffectiveEncounterCountsByMeaningsAsync(
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
				recentEncounters);
		}).ToList();

		return Result.Success<IReadOnlyList<SavedItemSummaryDto>>(dtos);
	}
}
