using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;

internal sealed class RecordWatchHeartbeatHandler
	: IRequestHandler<RecordWatchHeartbeatCommand, Result<RecordWatchHeartbeatResponseDto>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public RecordWatchHeartbeatHandler(
		IVideoRepository videoRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerVideoProgressRepository progressRepository,
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_transcriptRepository = transcriptRepository;
		_progressRepository = progressRepository;
		_encounterRepository = encounterRepository;
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

					var sevenDaysAgo = now.AddDays(-7);
					var touchedMeaningIds = new HashSet<KnowledgeMeaningId>();
					var effectiveMeaningDeltas = new Dictionary<KnowledgeMeaningId, int>();

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

							var delta = VideoTrackingHelper.ComputeSevenDayEncounterDelta(
								isNewRecord: true,
								previousLastEncounteredAt: null,
								incrementedEffective: true,
								currentEncounterCount: encounterRecord.EncounterCount,
								sevenDaysAgo: sevenDaysAgo);
							effectiveMeaningDeltas[km.Id] = effectiveMeaningDeltas.GetValueOrDefault(km.Id, 0) + delta;
						}
						else
						{
							var prevLastEncounteredAt = encounterRecord.LastEncounteredAt;
							var sourcesJson = VideoTrackingHelper.UpsertEncounterSource(
								encounterRecord.EncounterSourcesJson,
								sourceDto);

							bool incrementedEffective = encounterRecord.RecordEncounter(sourcesJson, now);
							var delta = VideoTrackingHelper.ComputeSevenDayEncounterDelta(
								isNewRecord: false,
								previousLastEncounteredAt: prevLastEncounteredAt,
								incrementedEffective: incrementedEffective,
								currentEncounterCount: encounterRecord.EncounterCount,
								sevenDaysAgo: sevenDaysAgo);
							if (delta > 0)
							{
								effectiveMeaningDeltas[km.Id] = effectiveMeaningDeltas.GetValueOrDefault(km.Id, 0) + delta;
							}
						}
					}

					// 3. Case 2A: Natural Exposure SRS DueAt Adjustment when 7-day effective encounters >= 3
					if (touchedMeaningIds.Count > 0)
					{
						var counts7d = await _encounterRepository.GetEffectiveEncounterCountsByMeaningsAsync(
							userId,
							touchedMeaningIds,
							sevenDaysAgo,
							cancellationToken);

						// Add unflushed deltas from the current Unit of Work to DB totals
						foreach (var (meaningId, delta) in effectiveMeaningDeltas)
						{
							counts7d[meaningId] = counts7d.GetValueOrDefault(meaningId, 0) + delta;
						}

						var qualifyingMeaningIds = counts7d
							.Where(kv => kv.Value >= 3)
							.Select(kv => kv.Key)
							.ToList();

						if (qualifyingMeaningIds.Count > 0)
						{
							var savedItems = await _savedItemRepository.GetByUserAndMeaningsWithScheduleAsync(
								userId,
								qualifyingMeaningIds,
								cancellationToken);

							var startOfToday = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
							var endOfToday = startOfToday.AddDays(1).AddTicks(-1);

							foreach (var savedItem in savedItems)
							{
								var schedule = savedItem.Schedule;
								if (schedule is null)
								{
									continue;
								}

								// Guard against shifting DueAt more than once on the same calendar day
								if (schedule.UpdatedAt >= startOfToday && schedule.DueAt > endOfToday)
								{
									continue;
								}

								var prevDueAt = schedule.DueAt;

								if (prevDueAt < startOfToday)
								{
									// Case 2.3 - TH 1 (Overdue): Grace rescue -> Now + 2 days (no lapse penalty)
									var newDueAt = now.AddDays(2);
									schedule.UpdateSchedule(newDueAt, now);
									srsAdjustedDtos.Add(new SrsAdjustedItemDto(
										savedItem.Id.Value,
										savedItem.Expression,
										"OverdueRescue",
										newDueAt));
								}
								else if (prevDueAt <= endOfToday)
								{
									// Case 2.3 - TH 2 (Due Today): Bonus interval -> Now + 4 days, IntervalDays += 4
									const int bonusDays = 4;
									var newDueAt = now.AddDays(bonusDays);
									schedule.UpdateSchedule(newDueAt, now);

									var latestDetail = schedule.Details
										.OrderByDescending(d => d.ReviewNumber)
										.FirstOrDefault();
									latestDetail?.AddBonusInterval(bonusDays);

									srsAdjustedDtos.Add(new SrsAdjustedItemDto(
										savedItem.Id.Value,
										savedItem.Expression,
										"DueTodayBonus",
										newDueAt));
								}
								// Case 2.3 - TH 3 (Not Yet Due): Keep DueAt unchanged; UI renders reinforcement badge from encounter count
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
