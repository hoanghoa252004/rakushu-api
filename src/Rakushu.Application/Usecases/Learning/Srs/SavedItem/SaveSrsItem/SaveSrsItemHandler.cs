using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.SaveSrsItem;

internal sealed class SaveSrsItemHandler : IRequestHandler<SaveSrsItemCommand, Result<Guid>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IKnowledgeMeaningRepository _knowledgeMeaningRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SaveSrsItemHandler(
		ISavedItemRepository savedItemRepository,
		IKnowledgeMeaningRepository knowledgeMeaningRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_knowledgeMeaningRepository = knowledgeMeaningRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		SaveSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaningId = KnowledgeMeaningId.From(request.MeaningId);
			var existingItem = await _savedItemRepository.GetByUserAndMeaningAsync(userId, meaningId, cancellationToken);
			if (existingItem is not null)
			{
				if (existingItem.IsActive)
				{
					return Result.Failure<Guid>(SavedItemErrors.AlreadyExists);
				}

				if (existingItem.IsRevoked)
				{
					return Result.Failure<Guid>(SavedItemErrors.CannotActivateRevokedMeaning);
				}

				existingItem.Activate();
				_savedItemRepository.Update(existingItem);
				return Result.Success(existingItem.Id.Value);
			}

			var meaning = await _knowledgeMeaningRepository.GetByIdAsync(meaningId, cancellationToken);
			if (meaning is null || !meaning.IsActive)
				return Result.Failure<Guid>(KnowledgeMeaningErrors.NotFound);

			var now = _systemClock.UtcNow;

			var savedItemResult = Domain.Entities.Srs.SavedItem.SavedItem.Create(
				userId,
				meaningId,
				now);

			if (savedItemResult.IsFailure)
				return Result.Failure<Guid>(savedItemResult.Error);

			var savedItem = savedItemResult.Value;
			var scheduleResult = SrsSchedule.Create(savedItem.Id, now);
			if (scheduleResult.IsFailure)
				return Result.Failure<Guid>(scheduleResult.Error);

			savedItem.AttachSchedule(scheduleResult.Value);
			_savedItemRepository.Add(savedItem);

			return Result.Success(savedItem.Id.Value);
		});
	}
}
