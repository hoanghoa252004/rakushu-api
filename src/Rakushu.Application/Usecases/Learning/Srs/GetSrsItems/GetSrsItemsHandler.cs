using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.GetSrsItems;

internal sealed class GetSrsItemsHandler : IRequestHandler<GetSrsItemsQuery, Result<IReadOnlyList<SrsItemDto>>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsItemsHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<IReadOnlyList<SrsItemDto>>> Handle(
		GetSrsItemsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<IReadOnlyList<SrsItemDto>>(UserErrors.NotFound);

		bool dueOnly = string.Equals(request.Status, "Due", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(request.Status, "Due_today", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(request.Status, "DueToday", StringComparison.OrdinalIgnoreCase);

		SrsStatus? statusFilter = null;
		if (!dueOnly && !string.IsNullOrWhiteSpace(request.Status) && !string.Equals(request.Status, "All", StringComparison.OrdinalIgnoreCase))
		{
			if (Enum.TryParse<SrsStatus>(request.Status, true, out var parsed) && parsed != SrsStatus.Ignored)
			{
				statusFilter = parsed;
			}
		}

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var items = await _savedItemRepository.GetUserItemsAsync(
			userId,
			statusFilter,
			dueOnly,
			page,
			pageSize,
			cancellationToken);

		var now = _systemClock.UtcNow;

		var dtos = items.Select(si => new SrsItemDto(
			si.Id.Value,
			si.KnowledgeMeaningId.Value,
			si.Expression,
			si.Reading,
			si.Meaning,
			si.Schedule?.ResolveStatus(now).ToString() ?? SrsStatus.Learning.ToString(),
			si.Schedule?.DueAt,
			si.Schedule?.LastReviewedAt,
			si.SavedAt)).ToList();

		return Result.Success<IReadOnlyList<SrsItemDto>>(dtos);
	}
}
