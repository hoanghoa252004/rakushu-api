using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVocabularySuggestions;

internal sealed class GetVocabularySuggestionsHandler
	: IRequestHandler<GetVocabularySuggestionsQuery, Result<IReadOnlyList<EncounteredVocabularySuggestionDto>>>
{
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IUserRepository _userRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetVocabularySuggestionsHandler(
		ILearnerEncounteredLearningUnitRepository encounterRepository,
		ISavedItemRepository savedItemRepository,
		IUserRepository userRepository,
		ICurrentUserContext currentUserContext)
	{
		_encounterRepository = encounterRepository;
		_savedItemRepository = savedItemRepository;
		_userRepository = userRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<IReadOnlyList<EncounteredVocabularySuggestionDto>>> Handle(
		GetVocabularySuggestionsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<IReadOnlyList<EncounteredVocabularySuggestionDto>>(UserErrors.NotFound);
		}

		int limit = Math.Clamp(request.Limit, 1, 50);

		var encounteredRows = await _encounterRepository.GetByUserWithDetailsAsync(userId, cancellationToken);
		if (encounteredRows.Count == 0)
		{
			return Result.Success<IReadOnlyList<EncounteredVocabularySuggestionDto>>([]);
		}

		var savedItems = await _savedItemRepository.GetUserItemsAsync(
			userId,
			page: 1,
			pageSize: 500,
			cancellationToken: cancellationToken);
		var savedMeaningIds = savedItems
			.Where(s => s.IsActive)
			.Select(s => s.KnowledgeMeaningId)
			.ToHashSet();

		var user = await _userRepository.GetProfileByIdAsync(userId, cancellationToken);
		var targetLevelOrder = user?.Profile?.Level?.SortOrder;
		var targetLevelCode = user?.Profile?.Level?.Code;
		var interestCategoryIds = user?.Profile?.Interests
			.Select(i => i.ContentCategoryId)
			.ToHashSet() ?? [];

		var groupedByMeaning = encounteredRows
			.Where(x => x.KnowledgeMeaning is not null
				&& x.KnowledgeMeaning.KnowledgeUnit is not null
				&& !savedMeaningIds.Contains(x.KnowledgeMeaningId))
			.GroupBy(x => x.KnowledgeMeaningId);

		var candidates = new List<EncounteredVocabularySuggestionDto>();

		foreach (var group in groupedByMeaning)
		{
			var rows = group.OrderByDescending(r => r.LastEncounteredAt).ToList();
			var primary = rows[0];
			var km = primary.KnowledgeMeaning;
			var ku = km.KnowledgeUnit;
			var levelCode = ku.ProficiencyLevel?.Code ?? "N3";
			var levelOrder = ku.ProficiencyLevel?.SortOrder ?? 3;

			int totalEffectiveEncounters = rows.Sum(r => r.EncounterCount);
			int totalRawPlaybacks = rows.Sum(r => r.RawPlaybackCount);
			int distinctVideoCount = rows.Select(r => r.VideoId.Value).Distinct().Count();

			// Spec Case 1.1.2: Standard Encounter Threshold
			// Criterion A: Appeared in >= 2 distinct videos (Cross-video)
			// Criterion B: Appeared in >= 3 distinct effective encounters (High Frequency)
			bool meetsCrossVideo = distinctVideoCount >= 2;
			bool meetsHighFrequency = totalEffectiveEncounters >= 3;

			if (!meetsCrossVideo && !meetsHighFrequency)
			{
				continue;
			}

			bool matchesInterest = interestCategoryIds.Count == 0
				|| rows.Any(r => r.Video is not null && interestCategoryIds.Contains(r.Video.ContentCategoryId));

			// Target level proximity: same level or +1 adjacent level (e.g. N3 learner -> N3 & N2)
			bool matchesLevel = !targetLevelOrder.HasValue
				|| Math.Abs(levelOrder - targetLevelOrder.Value) <= 1
				|| string.Equals(levelCode, targetLevelCode, StringComparison.OrdinalIgnoreCase);

			var mergedSources = new List<EncounterSourceDto>();
			foreach (var row in rows)
			{
				var parsedSources = VideoTrackingHelper.ParseEncounterSources(row.EncounterSourcesJson);
				if (parsedSources.Count > 0)
				{
					foreach (var src in parsedSources)
					{
						if (!mergedSources.Any(existing =>
							existing.VideoId == src.VideoId &&
							Math.Abs(existing.TimestampSeconds - src.TimestampSeconds) < 0.5))
						{
							mergedSources.Add(src);
						}
					}
				}
				else if (row.Video is not null)
				{
					var seg = row.LearningUnit?.TranscriptSegment;
					var ts = seg is not null ? Math.Round(seg.StartTime.TotalSeconds, 2) : 0;
					var sentence = seg?.Text ?? ku.Expression;
					if (!mergedSources.Any(existing =>
						existing.VideoId == row.Video.Id.Value &&
						Math.Abs(existing.TimestampSeconds - ts) < 0.5))
					{
						mergedSources.Add(new EncounterSourceDto(
							row.Video.Id.Value,
							row.Video.Title,
							ts,
							sentence));
					}
				}
			}

			string qualificationReason = meetsCrossVideo && meetsHighFrequency
				? $"Đã nghe {totalEffectiveEncounters} lần qua {distinctVideoCount} video khác nhau"
				: meetsCrossVideo
					? $"Xuất hiện xuyên suốt ở {distinctVideoCount} video bài học"
					: $"Từ khóa tần suất cao ({totalEffectiveEncounters} lần tiếp xúc độc lập)";

			var categoryName = rows
				.Select(r => r.Video?.ContentCategory?.Name)
				.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c));

			candidates.Add(new EncounteredVocabularySuggestionDto(
				km.Id.Value,
				ku.Id.Value,
				ku.Expression,
				ku.Reading,
				km.Meaning,
				levelCode,
				categoryName,
				matchesInterest,
				matchesLevel,
				totalEffectiveEncounters,
				totalRawPlaybacks,
				distinctVideoCount,
				qualificationReason,
				primary.LastEncounteredAt,
				mergedSources.Take(5).ToList()));
		}

		var ordered = candidates
			.OrderByDescending(c => (c.MatchesUserInterest ? 2 : 0) + (c.MatchesTargetLevel ? 2 : 0))
			.ThenByDescending(c => c.DistinctVideoCount)
			.ThenByDescending(c => c.TotalEncounterCount)
			.ThenByDescending(c => c.LastEncounteredAt)
			.Take(limit)
			.ToList();

		return Result.Success<IReadOnlyList<EncounteredVocabularySuggestionDto>>(ordered);
	}
}
