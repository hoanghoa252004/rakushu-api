using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

internal sealed class GetSrsItemsHandler : IRequestHandler<GetSrsItemsQuery, Result<IReadOnlyList<SavedItemSummaryDto>>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetSrsItemsHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
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
			request.IsActive,
			request.DueOnly,
			page,
			pageSize,
			cancellationToken);

		if (items.Count == 0)
			return Result.Success<IReadOnlyList<SavedItemSummaryDto>>(Array.Empty<SavedItemSummaryDto>());

		var dtos = items.Select(si => new SavedItemSummaryDto(
			si.Id.Value,
			si.KnowledgeMeaningId.Value,
			si.Expression,
			si.Reading,
			si.Meaning,
			si.IsActive,
			si.Schedule?.DueAt,
			si.SavedAt,
			si.IsRevoked)).ToList();

		return Result.Success<IReadOnlyList<SavedItemSummaryDto>>(dtos);
	}
}
