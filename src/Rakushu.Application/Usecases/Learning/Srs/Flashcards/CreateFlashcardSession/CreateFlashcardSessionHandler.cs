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
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;

internal sealed class CreateFlashcardSessionHandler : IRequestHandler<CreateFlashcardSessionCommand, Result<FlashcardSessionDetailDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateFlashcardSessionHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ISavedItemRepository savedItemRepository,
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_savedItemRepository = savedItemRepository;
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<FlashcardSessionDetailDto>> Handle(
		CreateFlashcardSessionCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<FlashcardSessionDetailDto>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			// Check for active session
			var activeSession = await _flashcardSessionRepository.GetActiveSessionByUserAsync(userId, cancellationToken);
			if (activeSession is not null)
			{
				if (activeSession.IsExpired(now))
				{
					activeSession.Cancel(now);
					_flashcardSessionRepository.Update(activeSession);
				}
				else
				{
					return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.ActiveSessionInProgress);
				}
			}

			// Validate input items
			var distinctItemIds = request.ItemIds.Distinct().Select(SavedItemId.From).ToList();
			if (distinctItemIds.Count == 0)
			{
				return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.EmptyItems);
			}

			// Retrieve saved items with schedules
			var savedItems = await _savedItemRepository.GetByIdsWithScheduleAsync(distinctItemIds, userId, cancellationToken);

			// Filter eligible items (not suspended)
			var eligibleItems = savedItems
				.Where(item => item.Schedule == null || item.Schedule.Status != SrsStatus.Suspended)
				.ToList();

			if (eligibleItems.Count == 0)
			{
				return Result.Failure<FlashcardSessionDetailDto>(FlashcardSessionErrors.EmptyItems);
			}

			// 1. Fetch multimedia context for segments and fallback meanings
			var segmentGuids = eligibleItems
				.Select(x => x.TranscriptSegmentId.Value)
				.Distinct()
				.ToList();

			var segmentContexts = segmentGuids.Count > 0
				? await _learningUnitRepository.GetMediaContextBySegmentIdsAsync(segmentGuids, cancellationToken)
				: new Dictionary<Guid, LearningUnitMediaContext>();

			var meaningGuids = eligibleItems.Select(x => x.KnowledgeMeaningId.Value).Distinct().ToList();
			var meaningContexts = await _learningUnitRepository.GetMediaContextByMeaningIdsAsync(meaningGuids, cancellationToken);

			// 2. Create Flashcard Session Aggregate
			var sessionResult = FlashcardSession.Create(userId, now);
			if (sessionResult.IsFailure)
				return Result.Failure<FlashcardSessionDetailDto>(sessionResult.Error);

			var session = sessionResult.Value;
			var itemDtos = new List<FlashcardItemDto>(eligibleItems.Count);

			foreach (var savedItem in eligibleItems)
			{
				LearningUnitMediaContext? media = null;
				if (segmentContexts.TryGetValue(savedItem.TranscriptSegmentId.Value, out var segMedia))
				{
					media = segMedia;
				}
				else
				{
					meaningContexts.TryGetValue(savedItem.KnowledgeMeaningId.Value, out media);
				}

				string? videoUrl = null;
				if (!string.IsNullOrWhiteSpace(media?.StorageKey))
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
				else if (!string.IsNullOrWhiteSpace(media?.SourceUrl))
				{
					videoUrl = media.SourceUrl;
				}

				var sessionItemResult = FlashcardSessionItem.Create(
					session.Id,
					savedItem.Id,
					savedItem.Expression,
					savedItem.Reading,
					savedItem.Meaning,
					media?.VideoTitle,
					media?.StorageKey,
					media?.SourceUrl,
					media?.StartTime,
					media?.EndTime,
					media?.OriginalSentence,
					media?.TranslatedSentence,
					media?.StartTokenIndex,
					media?.EndTokenIndex);

				if (sessionItemResult.IsFailure)
					return Result.Failure<FlashcardSessionDetailDto>(sessionItemResult.Error);

				session.AddItem(sessionItemResult.Value);

				FlashcardVideoDto? videoDto = null;
				if (media != null)
				{
					videoDto = new FlashcardVideoDto(
						media.VideoTitle,
						videoUrl,
						media.StartTime,
						media.EndTime);
				}

				FlashcardSubtitleDto? subtitleDto = null;
				if (!string.IsNullOrWhiteSpace(media?.OriginalSentence))
				{
					subtitleDto = new FlashcardSubtitleDto(
						media.OriginalSentence,
						media.TranslatedSentence,
						media.StartTokenIndex,
						media.EndTokenIndex);
				}

				itemDtos.Add(new FlashcardItemDto(
					savedItem.Id.Value,
					savedItem.Expression,
					savedItem.Reading,
					savedItem.Meaning,
					videoDto,
					subtitleDto));
			}

			_flashcardSessionRepository.Add(session);

			return Result.Success(new FlashcardSessionDetailDto(
				session.Id.Value,
				session.Status.ToString(),
				session.CreatedAt,
				session.ExpiresAt,
				session.CompletedAt,
				session.Items.Count,
				itemDtos));
		});
	}
}
