using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetKrashenVideoRecommendations;

internal sealed class GetKrashenVideoRecommendationsHandler
	: IRequestHandler<GetKrashenVideoRecommendationsQuery, Result<IReadOnlyList<KrashenVideoRecommendationDto>>>
{
	private const double IdealKnownRatio = 0.825; // Krashen i+1 sweet spot: 80% - 85% known
	private const int MaxComprehensibilityScore = 99;
	private const int FullReviewComprehensibilityScore = 75;
	private const int MinEffectiveEncounterCountForKnown = 3;
	private const int MinSrsRepetitionsForKnown = 2;
	private const int MinSrsIntervalDaysForKnown = 6;

	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ILearnerVideoProgressRepository _progressRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IUserRepository _userRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public GetKrashenVideoRecommendationsHandler(
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ILearnerVideoProgressRepository progressRepository,
		ISavedItemRepository savedItemRepository,
		IUserRepository userRepository,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_encounterRepository = encounterRepository;
		_progressRepository = progressRepository;
		_savedItemRepository = savedItemRepository;
		_userRepository = userRepository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<IReadOnlyList<KrashenVideoRecommendationDto>>> Handle(
		GetKrashenVideoRecommendationsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<IReadOnlyList<KrashenVideoRecommendationDto>>(UserErrors.NotFound);
		}

		int limit = Math.Clamp(request.Limit, 1, 20);

		var readyVideos = await _encounterRepository.GetReadyVideosWithLearningUnitsAsync(cancellationToken);
		if (readyVideos.Count == 0)
		{
			return Result.Success<IReadOnlyList<KrashenVideoRecommendationDto>>([]);
		}

		var encounteredRows = await _encounterRepository.GetByUserWithDetailsAsync(userId, cancellationToken);
		var savedItems = await _savedItemRepository.GetUserItemsAsync(
			userId,
			page: 1,
			pageSize: 1000,
			cancellationToken: cancellationToken);
		var progresses = await _progressRepository.GetByUserWithVideoDetailsAsync(userId, cancellationToken);
		var completedVideoIds = progresses
			.Where(p => p.IsCompleted)
			.Select(p => p.VideoId.Value)
			.ToHashSet();

		// Known vocabulary pool:
		// 1. Immersion: encountered effectively >= 3 times (summed across sentences for the same KnowledgeUnit)
		// 2. SRS: active SavedItem that graduated past initial cramming/lapses (>= 2 consecutive SM-2 reps or >= 6d interval)
		var knownByImmersion = encounteredRows
			.GroupBy(e => e.KnowledgeUnitId.Value)
			.Where(g => g.Sum(x => x.EncounterCount) >= MinEffectiveEncounterCountForKnown)
			.Select(g => g.Key);

		var knownBySrs = savedItems
			.Where(s => s.IsActive && s.KnowledgeMeaning?.KnowledgeUnitId is not null)
			.Where(s =>
			{
				var latestDetail = s.Schedule?.Details
					.OrderByDescending(d => d.ReviewNumber)
					.FirstOrDefault();
				return latestDetail is not null
					&& latestDetail.Quality >= 3
					&& (latestDetail.Repetitions >= MinSrsRepetitionsForKnown
						|| latestDetail.IntervalDays >= MinSrsIntervalDaysForKnown);
			})
			.Select(s => s.KnowledgeMeaning.KnowledgeUnitId.Value);

		var knownKnowledgeUnitIds = knownByImmersion
			.Concat(knownBySrs)
			.ToHashSet();

		// Cold-start guard (Option A): Krashen i+1 requires at least 1 known vocabulary unit
		// (encountered >= 3 times or graduated in SRS); brand-new learners receive empty list
		// so UI does not show misleading "0% known / 15% match" cards.
		if (knownKnowledgeUnitIds.Count == 0)
		{
			return Result.Success<IReadOnlyList<KrashenVideoRecommendationDto>>([]);
		}

		var user = await _userRepository.GetProfileByIdAsync(userId, cancellationToken);
		var interestCategoryIds = user?.Profile?.Interests
			.Select(i => i.ContentCategoryId)
			.ToHashSet() ?? [];

		var candidates = new List<(KrashenVideoRecommendationDto Dto, bool IsAlreadyCompleted, bool MatchesInterest)>();

		foreach (var video in readyVideos)
		{
			if (request.ExcludeVideoId.HasValue && video.Id.Value == request.ExcludeVideoId.Value)
			{
				continue;
			}

			var distinctVideoUnitIds = video.Transcript?.TranscriptSegments
				.SelectMany(ts => ts.LearningUnits)
				.Where(lu => lu.KnowledgeMeaning?.KnowledgeUnitId is not null)
				.Select(lu => lu.KnowledgeMeaning.KnowledgeUnitId.Value)
				.Distinct()
				.ToList() ?? [];

			int totalVocab = distinctVideoUnitIds.Count;
			if (totalVocab == 0)
			{
				continue;
			}

			int knownCount = distinctVideoUnitIds.Count(uid => knownKnowledgeUnitIds.Contains(uid));
			if (knownCount == 0)
			{
				continue;
			}

			int newCount = Math.Max(0, totalVocab - knownCount);
			double knownRatio = Math.Round((double)knownCount / totalVocab, 4);

			// Two-sided piecewise linear score around Krashen i+1 peak (0% -> 99% at 82.5% -> 75% at 100%)
			int comprehensibilityScore = CalculateComprehensibilityScore(knownRatio);

			string difficultyLabel;
			string reason;

			if (knownRatio >= 0.70 && knownRatio <= 0.92)
			{
				difficultyLabel = "Ideal";
				reason = $"{knownCount} từ quen thuộc giúp luyện phản xạ nghe tự nhiên • {newCount} từ mới lý tưởng để nâng trình (Krashen i+1)";
			}
			else if (knownRatio < 0.70)
			{
				difficultyLabel = "Challenging";
				reason = $"{newCount}/{totalVocab} từ mới ({Math.Round((1.0 - knownRatio) * 100)}%) — Thử thách mở rộng vốn từ chuyên sâu";
			}
			else
			{
				difficultyLabel = "Review";
				reason = $"Đã nắm {knownCount}/{totalVocab} từ ({Math.Round(knownRatio * 100)}%) — Phù hợp nghe thư giãn & củng cố tốc độ phản xạ";
			}

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

			double durationSecs = video.Duration.TotalSeconds;
			bool isCompleted = completedVideoIds.Contains(video.Id.Value);
			bool matchesInterest = interestCategoryIds.Contains(video.ContentCategoryId);

			var dto = new KrashenVideoRecommendationDto(
				video.Id.Value,
				video.Title,
				thumbnailUrl,
				Math.Round(durationSecs, 2),
				VideoTrackingHelper.FormatDuration(durationSecs),
				video.ContentCategory?.Name,
				video.Series?.Name,
				totalVocab,
				knownCount,
				newCount,
				knownRatio,
				comprehensibilityScore,
				difficultyLabel,
				reason);

			candidates.Add((dto, isCompleted, matchesInterest));
		}

		var ordered = candidates
			.OrderBy(c => c.IsAlreadyCompleted ? 1 : 0)
			.ThenByDescending(c => c.Dto.DifficultyLabel == "Ideal" ? 1 : 0)
			.ThenByDescending(c => c.Dto.ComprehensibilityScore + (c.MatchesInterest ? 5 : 0))
			.Take(limit)
			.Select(c => c.Dto)
			.ToList();

		return Result.Success<IReadOnlyList<KrashenVideoRecommendationDto>>(ordered);
	}

	private static int CalculateComprehensibilityScore(double knownRatio)
	{
		double clampedRatio = Math.Clamp(knownRatio, 0.0, 1.0);
		double rawScore = clampedRatio <= IdealKnownRatio
			? (clampedRatio / IdealKnownRatio) * MaxComprehensibilityScore
			: MaxComprehensibilityScore
				- ((clampedRatio - IdealKnownRatio) / (1.0 - IdealKnownRatio))
				* (MaxComprehensibilityScore - FullReviewComprehensibilityScore);

		return (int)Math.Clamp(Math.Round(rawScore), 0, MaxComprehensibilityScore);
	}
}
