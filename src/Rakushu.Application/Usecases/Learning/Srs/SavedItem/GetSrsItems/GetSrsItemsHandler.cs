using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

internal sealed class GetSrsItemsHandler : IRequestHandler<GetSrsItemsQuery, Result<IReadOnlyList<SrsItemDto>>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsItemsHandler(
		ISavedItemRepository savedItemRepository,
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
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

		if (items.Count == 0)
			return Result.Success<IReadOnlyList<SrsItemDto>>(Array.Empty<SrsItemDto>());

		var now = _systemClock.UtcNow;

		var meaningGuids = items
			.Select(si => si.KnowledgeMeaningId.Value)
			.Distinct()
			.ToList();

		var meaningContexts = await _learningUnitRepository.GetMediaContextsByMeaningIdsAsync(
			meaningGuids,
			maxVideosPerMeaning: 5,
			cancellationToken);

		var presignedUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		var dtos = new List<SrsItemDto>(items.Count);

		foreach (var si in items)
		{
			var videoContextDtos = new List<SrsItemVideoContextDto>();

			if (meaningContexts.TryGetValue(si.KnowledgeMeaningId.Value, out var mediaList))
			{
				foreach (var media in mediaList)
				{
					string? videoUrl = null;
					if (!string.IsNullOrWhiteSpace(media.StorageKey))
					{
						if (!presignedUrlCache.TryGetValue(media.StorageKey, out videoUrl))
						{
							try
							{
								videoUrl = await _storageService.CreatePresignedReadUrlAsync(media.StorageKey, cancellationToken);
								presignedUrlCache[media.StorageKey] = videoUrl;
							}
							catch
							{
								videoUrl = media.SourceUrl;
							}
						}
					}
					else if (!string.IsNullOrWhiteSpace(media.SourceUrl))
					{
						videoUrl = media.SourceUrl;
					}

					videoContextDtos.Add(new SrsItemVideoContextDto(
						media.VideoId,
						media.VideoTitle,
						videoUrl,
						media.StartTime,
						media.EndTime,
						media.OriginalSentence,
						media.TranslatedSentence,
						media.StartTokenIndex,
						media.EndTokenIndex,
						media.TranscriptSegmentId,
						media.LearningUnitId));
				}
			}

			dtos.Add(new SrsItemDto(
				si.Id.Value,
				si.KnowledgeMeaningId.Value,
				si.Expression,
				si.Reading,
				si.Meaning,
				si.Schedule?.ResolveStatus(now).ToString() ?? SrsStatus.Learning.ToString(),
				si.Schedule?.DueAt,
				si.Schedule?.LastReviewedAt,
				si.SavedAt,
				videoContextDtos));
		}

		return Result.Success<IReadOnlyList<SrsItemDto>>(dtos);
	}
}
