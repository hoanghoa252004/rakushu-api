using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;

internal sealed class GetFlashcardSessionHandler : IRequestHandler<GetFlashcardSessionQuery, Result<FlashcardSessionDetailDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_storageService = storageService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<FlashcardSessionDetailDto>> Handle(
		GetFlashcardSessionQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<FlashcardSessionDetailDto>(UserErrors.NotFound);

		var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
			FlashcardSessionId.From(request.SessionId),
			cancellationToken);

		if (session is null)
			return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.NotFound);

		if (session.UserId != userId)
			return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.NotFound);

		var now = _systemClock.UtcNow;
		if (session.IsExpired(now))
		{
			session.Cancel(now);
			_flashcardSessionRepository.Update(session);
			return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.Expired);
		}

		var itemDtos = new List<FlashcardItemDto>(session.Items.Count);

		foreach (var item in session.Items)
		{
			string? videoUrl = null;
			if (!string.IsNullOrWhiteSpace(item.StorageKey))
			{
				try
				{
					videoUrl = await _storageService.CreatePresignedReadUrlAsync(item.StorageKey, cancellationToken);
				}
				catch
				{
					videoUrl = item.VideoSourceUrl;
				}
			}
			else if (!string.IsNullOrWhiteSpace(item.VideoSourceUrl))
			{
				videoUrl = item.VideoSourceUrl;
			}

			FlashcardVideoDto? videoDto = null;
			if (!string.IsNullOrWhiteSpace(item.StorageKey) || !string.IsNullOrWhiteSpace(item.VideoSourceUrl))
			{
				videoDto = new FlashcardVideoDto(
					item.VideoTitle ?? string.Empty,
					videoUrl,
					item.StartTime,
					item.EndTime);
			}

			FlashcardSubtitleDto? subtitleDto = null;
			if (!string.IsNullOrWhiteSpace(item.OriginalSentence))
			{
				subtitleDto = new FlashcardSubtitleDto(
					item.OriginalSentence,
					item.TranslatedSentence,
					item.StartTokenIndex,
					item.EndTokenIndex);
			}

			itemDtos.Add(new FlashcardItemDto(
				item.SavedItemId.Value,
				item.Expression,
				item.Reading,
				item.Meaning,
				videoDto,
				subtitleDto));
		}

		return Result.Success(new FlashcardSessionDetailDto(
			session.Id.Value,
			session.Status.ToString(),
			session.CreatedAt,
			session.ExpiresAt,
			session.CompletedAt,
			session.Items.Count,
			itemDtos));
	}
}
