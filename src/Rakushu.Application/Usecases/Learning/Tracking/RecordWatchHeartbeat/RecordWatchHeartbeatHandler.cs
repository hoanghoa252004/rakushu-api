using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;

internal sealed class RecordWatchHeartbeatHandler
	: IRequestHandler<RecordWatchHeartbeatCommand, Result<RecordWatchHeartbeatResponseDto>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ILearnerEncounterEventRepository _encounterEventRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public RecordWatchHeartbeatHandler(
		IVideoRepository videoRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerVideoProgressRepository progressRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ILearnerEncounterEventRepository encounterEventRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_transcriptRepository = transcriptRepository;
		_progressRepository = progressRepository;
		_encounterRepository = encounterRepository;
		_encounterEventRepository = encounterEventRepository;
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RecordWatchHeartbeatResponseDto>> Handle(
		RecordWatchHeartbeatCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<RecordWatchHeartbeatResponseDto>(UserErrors.NotFound);
		}

		var videoId = VideoId.From(request.VideoId);
		var video = await _videoRepository.GetByIdAsync(videoId, cancellationToken);
		if (video is null)
		{
			return Result.Failure<RecordWatchHeartbeatResponseDto>(VideoErrors.NotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;
			if (request.VideoDurationSeconds is > 1.0)
			{
				video.SyncActualDuration(TimeSpan.FromSeconds(request.VideoDurationSeconds.Value), now);
			}

			var videoDurationSeconds = video.Duration.TotalSeconds;

			// 1. Upsert LearnerVideoProgress & merge WatchedIntervals (Interval Union)
			var progress = await _progressRepository.GetByUserAndVideoAsync(userId, videoId, cancellationToken);
			if (progress is null)
			{
				var createResult = LearnerVideoProgress.Create(userId, videoId, now);
				if (createResult.IsFailure)
				{
					return Result.Failure<RecordWatchHeartbeatResponseDto>(createResult.Error);
				}

				progress = createResult.Value;
				_progressRepository.Add(progress);
			}

			bool isRangeValidForEncounterScan = progress.RecordPlayback(
				request.CurrentPositionSeconds,
				request.WatchedRange?.StartSeconds,
				request.WatchedRange?.EndSeconds,
				videoDurationSeconds,
				now);

			var srsAdjustedDtos = new List<SrsAdjustedItemDto>();

			// 2. Scan overlapping TranscriptSegments & LearningUnits only if Dwell Guard (>= 1.0s) passed
			if (isRangeValidForEncounterScan && request.WatchedRange is not null)
			{
				var rangeStart = TimeSpan.FromSeconds(Math.Max(0, request.WatchedRange.StartSeconds));
				var rangeEnd = TimeSpan.FromSeconds(Math.Max(rangeStart.TotalSeconds, request.WatchedRange.EndSeconds));

				var overlappingSegments = await _transcriptRepository.GetOverlappingSegmentsWithLearningUnitsAsync(
					videoId,
					rangeStart,
					rangeEnd,
					cancellationToken);

				var candidateUnits = overlappingSegments
					.SelectMany(seg => seg.LearningUnits.Select(lu => (Segment: seg, Unit: lu)))
					.Where(x => x.Unit.KnowledgeMeaning is not null && x.Unit.KnowledgeMeaning.KnowledgeUnit is not null)
					.GroupBy(x => x.Unit.Id)
					.Select(g => g.First())
					.ToList();

				if (candidateUnits.Count > 0)
				{
					var unitIds = candidateUnits.Select(x => x.Unit.Id).ToList();
					var existingEncounters = await _encounterRepository.GetByUserAndLearningUnitIdsAsync(
						userId,
						unitIds,
						cancellationToken);
					var existingByUnitId = existingEncounters.ToDictionary(x => x.LearningUnitId);

					var sevenDaysAgo = now.AddDays(-SrsSchedule.ImmersionWindowDays);
					var touchedMeaningIds = new HashSet<KnowledgeMeaningId>();
					var unflushedEventCounts = new Dictionary<KnowledgeMeaningId, int>();

					foreach (var (segment, lu) in candidateUnits)
					{
						var km = lu.KnowledgeMeaning;
						var ku = km.KnowledgeUnit;
						var sourceDto = new EncounterSourceDto(
							video.Id.Value,
							video.Title,
							Math.Round(segment.StartTime.TotalSeconds, 2),
							segment.Text);

						touchedMeaningIds.Add(km.Id);
						bool recordedNewEffectiveEncounter;

						if (!existingByUnitId.TryGetValue(lu.Id, out var encounterRecord))
						{
							var sourcesJson = VideoTrackingHelper.UpsertEncounterSource("[]", sourceDto);
							var createEncounterResult = LearnerEncounteredLearningUnit.Create(
								userId,
								lu.Id,
								km.Id,
								ku.Id,
								videoId,
								sourcesJson,
								now);

							if (createEncounterResult.IsFailure)
							{
								return Result.Failure<RecordWatchHeartbeatResponseDto>(createEncounterResult.Error);
							}

							encounterRecord = createEncounterResult.Value;
							_encounterRepository.Add(encounterRecord);
							existingByUnitId[lu.Id] = encounterRecord;
							recordedNewEffectiveEncounter = true;
						}
						else
						{
							var sourcesJson = VideoTrackingHelper.UpsertEncounterSource(
								encounterRecord.EncounterSourcesJson,
								sourceDto);

							recordedNewEffectiveEncounter = encounterRecord.RecordEncounter(sourcesJson, now);
						}

						if (recordedNewEffectiveEncounter)
						{
							var eventResult = LearnerEncounterEvent.Create(
								userId,
								lu.Id,
								km.Id,
								videoId,
								now);

							if (eventResult.IsFailure)
							{
								return Result.Failure<RecordWatchHeartbeatResponseDto>(eventResult.Error);
							}

							_encounterEventRepository.Add(eventResult.Value);
							unflushedEventCounts[km.Id] = unflushedEventCounts.GetValueOrDefault(km.Id, 0) + 1;
						}
					}

					// 3. Case 2A: Natural Exposure SRS DueAt Adjustment
					// Uses append-only encounter events, per-schedule watermark (no double-dipping),
					// and max consecutive passive boost cap (passive listening cannot permanently replace active recall).
					if (touchedMeaningIds.Count > 0)
					{
						var savedItems = await _savedItemRepository.GetByUserAndMeaningsWithScheduleAsync(
							userId,
							touchedMeaningIds,
							cancellationToken);

						var startOfToday = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
						var endOfToday = startOfToday.AddDays(1).AddTicks(-1);

						var eligibleSavedItems = savedItems
							.Where(si => si.Schedule is not null && si.Schedule.CanReceivePassiveBoost(startOfToday, endOfToday))
							.ToList();

						if (eligibleSavedItems.Count > 0)
						{
							var exclusiveAfterByMeaning = new Dictionary<KnowledgeMeaningId, DateTimeOffset?>();
							foreach (var savedItem in eligibleSavedItems)
							{
								exclusiveAfterByMeaning[savedItem.KnowledgeMeaningId] = savedItem.Schedule!.GetPassiveBoostExclusiveAfterUtc();
							}

							var dbWindowCounts = await _encounterEventRepository.GetEncounterCountsInWindowAsync(
								userId,
								exclusiveAfterByMeaning,
								sevenDaysAgo,
								cancellationToken);

							foreach (var savedItem in eligibleSavedItems)
							{
								var schedule = savedItem.Schedule!;
								var exclusiveAfter = exclusiveAfterByMeaning[savedItem.KnowledgeMeaningId];

								int dbCount = dbWindowCounts.GetValueOrDefault(savedItem.KnowledgeMeaningId, 0);
								int unflushedCount = (!exclusiveAfter.HasValue || now > exclusiveAfter.Value)
									? unflushedEventCounts.GetValueOrDefault(savedItem.KnowledgeMeaningId, 0)
									: 0;

								int effectiveWindowCount = dbCount + unflushedCount;
								if (effectiveWindowCount < SrsSchedule.PassiveBoostEncounterThreshold)
								{
									continue;
								}

								var prevDueAt = schedule.DueAt;

								if (prevDueAt < startOfToday)
								{
									// Case 2.3 - TH 1 (Overdue): Grace rescue -> Now + 2 days (no lapse penalty)
									var newDueAt = now.AddDays(SrsSchedule.OverdueRescueDays);
									var boostResult = schedule.ApplyPassiveImmersionBoost(
										newDueAt,
										SrsImmersionAdjustmentTypes.OverdueRescue,
										SrsSchedule.OverdueRescueDays,
										effectiveWindowCount,
										now);
									if (boostResult.IsFailure)
									{
										return Result.Failure<RecordWatchHeartbeatResponseDto>(boostResult.Error);
									}

									srsAdjustedDtos.Add(new SrsAdjustedItemDto(
										savedItem.Id.Value,
										savedItem.Expression,
										SrsImmersionAdjustmentTypes.OverdueRescue,
										newDueAt));
								}
								else if (prevDueAt <= endOfToday)
								{
									// Case 2.3 - TH 2 (Due Today): Soft defer -> Now + 4 days
									// Active-recall IntervalDays stays untouched so passive watching cannot inflate mastery status.
									var newDueAt = now.AddDays(SrsSchedule.DueTodayBonusDays);
									var boostResult = schedule.ApplyPassiveImmersionBoost(
										newDueAt,
										SrsImmersionAdjustmentTypes.DueTodayBonus,
										SrsSchedule.DueTodayBonusDays,
										effectiveWindowCount,
										now);
									if (boostResult.IsFailure)
									{
										return Result.Failure<RecordWatchHeartbeatResponseDto>(boostResult.Error);
									}

									srsAdjustedDtos.Add(new SrsAdjustedItemDto(
										savedItem.Id.Value,
										savedItem.Expression,
										SrsImmersionAdjustmentTypes.DueTodayBonus,
										newDueAt));
								}
							}
						}
					}
				}
			}

			return Result.Success(new RecordWatchHeartbeatResponseDto(
				progress.Id.Value,
				srsAdjustedDtos.Count,
				srsAdjustedDtos));
		}, cancellationToken);
	}
}
