using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;

internal sealed class GetFlashcardSessionHandler : IRequestHandler<GetFlashcardSessionQuery, Result<FlashcardSessionDetailDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public GetFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
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
			if (!session.HasAnyReviewedItems)
			{
				_flashcardSessionRepository.Delete(session);
			}
			else
			{
				var unrated = session.ExpireAndPrune(now);
				if (unrated.Count > 0)
				{
					_flashcardSessionRepository.RemoveItems(unrated);
				}
				_flashcardSessionRepository.Update(session);
			}
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.Expired);
		}

		if (session.Status == FlashcardSessionStatus.InProgress)
		{
			session.RefreshIdleTimeout(now);
			_flashcardSessionRepository.Update(session);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		var meaningGuids = session.Items
			.Where(i => i.SavedItem != null)
			.Select(i => i.SavedItem.KnowledgeMeaningId.Value)
			.Distinct()
			.ToList();

		var meaningContexts = await _learningUnitRepository.GetMediaContextsByMeaningIdsAsync(meaningGuids, maxVideosPerMeaning: 5, cancellationToken);

		var itemDtos = new List<FlashcardItemDto>(session.Items.Count);

		foreach (var item in session.Items)
		{
			var savedItem = item.SavedItem;
			if (savedItem is null)
				continue;

			var videoContextDtos = new List<FlashcardMediaContextDto>();
			if (meaningContexts.TryGetValue(savedItem.KnowledgeMeaningId.Value, out var mediaList))
			{
				foreach (var media in mediaList)
				{
					string? videoUrl = null;
					if (!string.IsNullOrWhiteSpace(media.StorageKey))
					{
						try
						{
							videoUrl = await _storageService.CreatePresignedReadUrlAsync(media.StorageKey, cancellationToken);
						}
						catch
						{
							videoUrl = media.SourceUrl;
						}
					}
					else if (!string.IsNullOrWhiteSpace(media.SourceUrl))
					{
						videoUrl = media.SourceUrl;
					}

					videoContextDtos.Add(new FlashcardMediaContextDto(
						media.LearningUnitId ?? Guid.Empty,
						media.VideoTitle,
						videoUrl,
						media.StartTime,
						media.EndTime,
						media.OriginalSentence,
						media.TranslatedSentence,
						media.StartTokenIndex,
						media.EndTokenIndex));
				}
			}

			itemDtos.Add(new FlashcardItemDto(
				item.Id.Value,
				savedItem.Id.Value,
				savedItem.Expression,
				savedItem.Reading,
				savedItem.Meaning,
				item.Rating?.ToString(),
				item.ReviewedAt,
				videoContextDtos));
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
