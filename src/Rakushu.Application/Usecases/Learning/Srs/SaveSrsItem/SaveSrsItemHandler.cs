using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.SaveSrsItem;

internal sealed class SaveSrsItemHandler : IRequestHandler<SaveSrsItemCommand, Result<SrsItemDto>>
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

	public async Task<Result<SrsItemDto>> Handle(
		SaveSrsItemCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<SrsItemDto>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaningId = KnowledgeMeaningId.From(request.MeaningId);
			var existingItem = await _savedItemRepository.GetByUserAndMeaningAsync(userId, meaningId, cancellationToken);
			var now = _systemClock.UtcNow;

			if (existingItem is not null)
			{
				return Result.Success(new SrsItemDto(
					existingItem.Id.Value,
					existingItem.KnowledgeMeaningId.Value,
					existingItem.Expression,
					existingItem.Reading,
					existingItem.Meaning,
					existingItem.Schedule?.ResolveStatus(now).ToString() ?? SrsStatus.Learning.ToString(),
					existingItem.Schedule?.DueAt,
					existingItem.Schedule?.LastReviewedAt,
					existingItem.SavedAt));
			}

			var meaning = await _knowledgeMeaningRepository.GetByIdAsync(meaningId, cancellationToken);
			if (meaning is null)
				return Result.Failure<SrsItemDto>(KnowledgeMeaningErrors.NotFound);

			var expression = meaning.KnowledgeUnit?.Expression ?? meaning.Meaning;
			var reading = meaning.KnowledgeUnit?.Reading;
			var definition = meaning.Meaning;

			var savedItemResult = SavedItem.Create(
				userId,
				meaningId,
				expression,
				reading,
				definition,
				now);

			if (savedItemResult.IsFailure)
				return Result.Failure<SrsItemDto>(savedItemResult.Error);

			var savedItem = savedItemResult.Value;
			var scheduleResult = SrsSchedule.Create(savedItem.Id, now);
			if (scheduleResult.IsFailure)
				return Result.Failure<SrsItemDto>(scheduleResult.Error);

			savedItem.AttachSchedule(scheduleResult.Value);
			_savedItemRepository.Add(savedItem);

			return Result.Success(new SrsItemDto(
				savedItem.Id.Value,
				savedItem.KnowledgeMeaningId.Value,
				savedItem.Expression,
				savedItem.Reading,
				savedItem.Meaning,
				SrsStatus.Learning.ToString(),
				scheduleResult.Value.DueAt,
				null,
				savedItem.SavedAt));
		});
	}
}
