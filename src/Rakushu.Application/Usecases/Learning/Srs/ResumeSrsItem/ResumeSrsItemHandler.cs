using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

internal sealed class ResumeSrsItemHandler : IRequestHandler<ResumeSrsItemCommand, Result<SrsItemDto>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ResumeSrsItemHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<SrsItemDto>> Handle(
		ResumeSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<SrsItemDto>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var itemId = SavedItemId.From(request.Id);
			var item = await _savedItemRepository.GetByIdWithScheduleAsync(itemId, cancellationToken);
			if (item is null || item.UserId != userId)
				return Result.Failure<SrsItemDto>(SavedItemErrors.NotFound);

			if (item.Schedule is null)
				return Result.Failure<SrsItemDto>(SavedItemErrors.NotFound);

			var now = _systemClock.UtcNow;
			var latestDetail = item.Schedule.Details.OrderByDescending(d => d.ReviewNumber).FirstOrDefault();
			int repetitions = latestDetail?.Repetitions ?? 0;
			var targetStatus = repetitions >= 3 ? SrsStatus.Review : SrsStatus.Learning;

			var resumeResult = item.Schedule.Resume(targetStatus, now);
			if (resumeResult.IsFailure)
				return Result.Failure<SrsItemDto>(resumeResult.Error);

			return Result.Success(new SrsItemDto(
				item.Id.Value,
				item.KnowledgeMeaningId.Value,
				item.Expression,
				item.Reading,
				item.Meaning,
				item.Schedule.ResolveStatus(now).ToString(),
				item.Schedule.DueAt,
				item.Schedule.LastReviewedAt,
				item.CreatedAt));
		}, cancellationToken);
	}
}
