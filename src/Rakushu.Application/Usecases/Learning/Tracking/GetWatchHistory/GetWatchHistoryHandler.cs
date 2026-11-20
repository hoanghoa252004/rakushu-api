using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetWatchHistory;

internal sealed class GetWatchHistoryHandler
	: IRequestHandler<GetWatchHistoryQuery, Result<IReadOnlyList<WatchedVideoHistoryItemDto>>>
{
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public GetWatchHistoryHandler(
		ILearnerVideoProgressRepository progressRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_progressRepository = progressRepository;
		_encounterRepository = encounterRepository;
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<IReadOnlyList<WatchedVideoHistoryItemDto>>> Handle(
		GetWatchHistoryQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<IReadOnlyList<WatchedVideoHistoryItemDto>>(UserErrors.NotFound);
		}

		var progresses = await _progressRepository.GetByUserWithVideoDetailsAsync(userId, cancellationToken);
		if (progresses.Count == 0)
		{
			return Result.Success<IReadOnlyList<WatchedVideoHistoryItemDto>>([]);
		}

		var encounteredRows = await _encounterRepository.GetByUserWithDetailsAsync(userId, cancellationToken);
		var encounterByUnitId = encounteredRows
			.GroupBy(x => x.LearningUnitId)
			.ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.LastEncounteredAt).First());

		var savedItems = await _savedItemRepository.GetUserItemsAsync(
			userId,
			page: 1,
			pageSize: 500,
			cancellationToken: cancellationToken);
		var savedItemByMeaningId = savedItems
			.Where(s => s.IsActive)
			.GroupBy(s => s.KnowledgeMeaningId)
			.ToDictionary(g => g.Key, g => g.First().Id.Value);

		var dtos = new List<WatchedVideoHistoryItemDto>(progresses.Count);

		foreach (var progress in progresses)
		{
			var video = progress.Video;
			if (video is null)
			{
				continue;
			}

			double durationSecs = video.Duration.TotalSeconds > 0
				? video.Duration.TotalSeconds
				: Math.Max(60, progress.TotalWatchedSeconds);

			string? thumbnailUrl = null;
			var thumbAsset = video.MediaAssets
				.FirstOrDefault(a => a.AssetType == MediaAssetType.Thumbnail && !string.IsNullOrEmpty(a.StorageKey));
			if (thumbAsset?.StorageKey is not null)
			{
				if (thumbAsset.StorageKey.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
					|| thumbAsset.StorageKey.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
				{
					thumbnailUrl = thumbAsset.StorageKey;
				}
				else
				{
					try
					{
						thumbnailUrl = await _storageService.CreatePresignedReadUrlAsync(thumbAsset.StorageKey, cancellationToken);
					}
					catch
					{
						thumbnailUrl = null;
					}
				}
			}

			var intervals = progress.GetWatchedIntervals();

			var rawSegments = video.Transcript?.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.ToList() ?? [];

			var unitDtos = new List<WatchHistoryLearningUnitDto>();
			foreach (var seg in rawSegments)
			{
				double segStart = Math.Round(seg.StartTime.TotalSeconds, 2);
				double segEnd = Math.Round(seg.EndTime.TotalSeconds, 2);

				foreach (var lu in seg.LearningUnits)
				{
					var km = lu.KnowledgeMeaning;
					var ku = km?.KnowledgeUnit;
					if (km is null || ku is null)
					{
						continue;
					}

					bool hasDbEncounter = encounterByUnitId.TryGetValue(lu.Id, out var encRecord);
					bool intersectsInterval = IntersectsAnyInterval(segStart, segEnd, intervals);
					bool isEncountered = hasDbEncounter || intersectsInterval;
					int encounterCount = hasDbEncounter
						? encRecord!.EncounterCount
						: (intersectsInterval ? 1 : 0);
					int rawPlaybackCount = hasDbEncounter
						? encRecord!.RawPlaybackCount
						: encounterCount;

					bool isSaved = savedItemByMeaningId.TryGetValue(km.Id, out var savedItemId);

					unitDtos.Add(new WatchHistoryLearningUnitDto(
						lu.Id.Value,
						km.Id.Value,
						ku.Expression,
						ku.Reading,
						km.Meaning,
						ku.ProficiencyLevel?.Code ?? "N3",
						ku.KnowledgeType.ToString(),
						segStart,
						segEnd,
						seg.Text,
						isEncountered,
						encounterCount,
						rawPlaybackCount,
						isSaved,
						isSaved ? savedItemId : null));
				}
			}

			int totalUnitsCount = unitDtos.Count;
			int encounteredUnitsCount = unitDtos.Count(u => u.IsEncountered);
			int absorptionPercent = totalUnitsCount > 0
				? (int)Math.Round((encounteredUnitsCount * 100.0) / totalUnitsCount)
				: 0;
			double effectiveCompletionRate = durationSecs > 0
				? Math.Max(progress.CompletionRate, Math.Min(1.0, progress.TotalWatchedSeconds / durationSecs))
				: progress.CompletionRate;
			int coveragePercent = (int)Math.Clamp(Math.Round(effectiveCompletionRate * 100.0), 0, 100);
			double clampedEffectiveWatched = Math.Min(progress.TotalWatchedSeconds, durationSecs);

			dtos.Add(new WatchedVideoHistoryItemDto(
				video.Id.Value,
				video.Slug,
				video.Title,
				video.Series?.Name,
				thumbnailUrl,
				Math.Round(durationSecs, 2),
				VideoTrackingHelper.FormatDuration(durationSecs),
				video.SeriesId?.Value,
				video.Series?.Name,
				video.ContentCategory?.Name,
				progress.LastWatchedAt,
				progress.LastWatchedSecond,
				intervals,
				clampedEffectiveWatched,
				coveragePercent,
				totalUnitsCount,
				encounteredUnitsCount,
				absorptionPercent,
				unitDtos));
		}

		return Result.Success<IReadOnlyList<WatchedVideoHistoryItemDto>>(dtos);
	}

	private static bool IntersectsAnyInterval(double segStart, double segEnd, IReadOnlyList<double[]> intervals)
	{
		var effectiveEnd = segEnd > segStart ? segEnd : segStart + 3.0;
		foreach (var iv in intervals)
		{
			if (iv.Length < 2)
			{
				continue;
			}

			var overlapStart = Math.Max(segStart, iv[0]);
			var overlapEnd = Math.Min(effectiveEnd, iv[1]);
			if (overlapStart < overlapEnd)
			{
				return true;
			}
		}

		return false;
	}
}
