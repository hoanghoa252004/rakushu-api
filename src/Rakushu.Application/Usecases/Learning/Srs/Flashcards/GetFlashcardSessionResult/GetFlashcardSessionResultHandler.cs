using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessionResult;

internal sealed class GetFlashcardSessionResultHandler : IRequestHandler<GetFlashcardSessionResultQuery, Result<FlashcardSessionResultDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetFlashcardSessionResultHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ICurrentUserContext currentUserContext)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<FlashcardSessionResultDto>> Handle(
		GetFlashcardSessionResultQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<FlashcardSessionResultDto>(UserErrors.NotFound);

		var session = await _flashcardSessionRepository.GetResultByIdAsync(
			FlashcardSessionId.From(request.SessionId),
			cancellationToken);

		if (session is null)
			return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotFound);

		if (session.UserId != userId)
			return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotFound);

		if (session.Status != FlashcardSessionStatus.Completed)
			return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotCompleted);

		int forgotCount = session.Items.Count(i => i.Rating == FlashcardRating.Forgot);
		int hardCount = session.Items.Count(i => i.Rating == FlashcardRating.Hard);
		int easyCount = session.Items.Count(i => i.Rating == FlashcardRating.Easy);

		var results = new List<FlashcardItemResultDto>(session.Items.Count);

		foreach (var item in session.Items)
		{
			var savedItem = item.SavedItem;
			if (savedItem is null)
				continue;

			var detail = savedItem.Schedule?.Details.FirstOrDefault(d => d.FlashcardSessionItemId == item.Id);
			int quality = detail?.Quality ?? (int)(item.Rating ?? FlashcardRating.Easy);
			int intervalDays = detail?.IntervalDays ?? 1;
			int repetitions = detail?.Repetitions ?? 1;
			DateTimeOffset nextDueAt = savedItem.Schedule?.DueAt ?? (item.ReviewedAt ?? session.CompletedAt ?? DateTimeOffset.UtcNow).AddDays(intervalDays);

			results.Add(new FlashcardItemResultDto(
				savedItem.Id.Value,
				savedItem.Expression,
				savedItem.Reading,
				savedItem.Meaning,
				item.Rating?.ToString() ?? "Easy",
				quality,
				intervalDays,
				repetitions,
				nextDueAt));
		}

		return Result.Success(new FlashcardSessionResultDto(
			session.Id.Value,
			session.Status.ToString(),
			session.CompletedAt,
			session.Items.Count,
			forgotCount,
			hardCount,
			easyCount,
			results));
	}
}
