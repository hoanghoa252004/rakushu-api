using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;

using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.EnrollLearningUnit;

internal sealed class EnrollLearningUnitHandler : IRequestHandler<EnrollLearningUnitCommand, Result<EnrollLearningUnitResponse>>
{
	private readonly ICurrentUserContext _currentUser;
	private readonly ISrsNoteRepository _noteRepository;
	private readonly ISrsCardRepository _cardRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public EnrollLearningUnitHandler(
		ICurrentUserContext currentUser,
		ISrsNoteRepository noteRepository,
		ISrsCardRepository cardRepository,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_noteRepository = noteRepository;
		_cardRepository = cardRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result<EnrollLearningUnitResponse>> Handle(
		EnrollLearningUnitCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var userId = _currentUser.UserId;
			if (userId is null)
				return Result.Failure<EnrollLearningUnitResponse>(Rakushu.Domain.Entities.Identity.User.UserErrors.UnauthorizedResourceAccess);

			var learningUnitId = LearningUnitId.From(request.LearningUnitId);
			if (!await _noteRepository.LearningUnitExistsAsync(learningUnitId, cancellationToken))
				return Result.Failure<EnrollLearningUnitResponse>(LearningUnitErrors.NotFound);

			var existing = await _noteRepository.GetEnrollmentAsync(userId, learningUnitId, cancellationToken);
			if (existing.Note is not null)
			{
				if (existing.Card?.State is null)
					return Result.Failure<EnrollLearningUnitResponse>(SrsNoteErrors.EnrollmentIntegrityConflict);

				return Result.Success(new EnrollLearningUnitResponse(
					existing.Note.Id.Value,
					existing.Card.Id.Value,
					existing.Card.Status.ToString(),
					existing.Card.State.DueAt,
					true));
			}

			var now = _clock.UtcNow;
			var note = SrsNote.Create(userId, learningUnitId, now);
			var cardResult = SrsCard.Create(note.Id, now);
			if (cardResult.IsFailure)
				return Result.Failure<EnrollLearningUnitResponse>(cardResult.Error);

			var card = cardResult.Value;
			var state = SrsCardState.Create(card.Id, now);
			var attachResult = card.AttachInitialState(state, now);
			if (attachResult.IsFailure)
				return Result.Failure<EnrollLearningUnitResponse>(attachResult.Error);

			_noteRepository.Add(note);
			_cardRepository.Add(card);
			_cardRepository.AddState(state);

			return Result.Success(new EnrollLearningUnitResponse(
				note.Id.Value,
				card.Id.Value,
				card.Status.ToString(),
				state.DueAt,
				false));
		}, cancellationToken);
	}
}