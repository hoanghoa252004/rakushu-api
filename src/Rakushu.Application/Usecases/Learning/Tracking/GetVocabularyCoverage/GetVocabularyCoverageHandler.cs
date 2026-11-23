using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVocabularyCoverage;

internal sealed class GetVocabularyCoverageHandler
	: IRequestHandler<GetVocabularyCoverageQuery, Result<VocabularyCoverageOverviewDto>>
{
	private readonly ILearnerEncounteredLearningUnitRepository _encounterRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IUserRepository _userRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetVocabularyCoverageHandler(
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

	public async Task<Result<VocabularyCoverageOverviewDto>> Handle(
		GetVocabularyCoverageQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<VocabularyCoverageOverviewDto>(UserErrors.NotFound);
		}

		var encounteredRows = await _encounterRepository.GetByUserWithDetailsAsync(userId, cancellationToken);
		var savedItems = await _savedItemRepository.GetUserItemsAsync(
			userId,
			page: 1,
			pageSize: 500,
			cancellationToken: cancellationToken);
		var user = await _userRepository.GetProfileByIdAsync(userId, cancellationToken);
		var targetLevelCode = user?.Profile?.Level?.Code ?? "N3";

		var systemLevelCounts = await _encounterRepository.GetSystemVocabularyCountsByLevelAsync(cancellationToken);
		var systemCategoryUnits = await _encounterRepository.GetSystemVocabularyByCategoryAsync(cancellationToken);

		// 1. Unique encountered KnowledgeUnits grouped by JLPT Level Code
		var encounteredUnitIdsByLevel = encounteredRows
			.Where(e => e.KnowledgeMeaning?.KnowledgeUnit?.ProficiencyLevel is not null)
			.GroupBy(e => e.KnowledgeMeaning.KnowledgeUnit.ProficiencyLevel.Code, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				g => g.Key,
				g => g.Select(e => e.KnowledgeUnitId.Value).Distinct().Count(),
				StringComparer.OrdinalIgnoreCase);

		var levelCoverages = systemLevelCounts
			.Select(lvl =>
			{
				int encountered = encounteredUnitIdsByLevel.GetValueOrDefault(lvl.LevelCode, 0);
				int total = Math.Max(encountered, lvl.TotalUnitsCount);
				int pct = total > 0 ? (int)Math.Clamp(Math.Round((encountered * 100.0) / total), 0, 100) : 0;
				return new JlptLevelCoverageDto(
					lvl.LevelCode,
					lvl.LevelName,
					lvl.SortOrder,
					encountered,
					total,
					pct);
			})
			.OrderBy(l => l.SortOrder)
			.ToList();

		// 2. Exposure Depth Buckets (Spec Case 4.2.2)
		var activeSavedItems = savedItems.Where(s => s.IsActive).ToList();
		var savedMeaningIds = activeSavedItems.Select(s => s.KnowledgeMeaningId).ToHashSet();

		var unsavedByMeaning = encounteredRows
			.Where(e => !savedMeaningIds.Contains(e.KnowledgeMeaningId))
			.GroupBy(e => e.KnowledgeMeaningId)
			.ToList();

		int justHeardOnceCount = 0;
		int highFrequencyUnsavedCount = 0;

		foreach (var grp in unsavedByMeaning)
		{
			int totalEncounters = grp.Sum(x => x.EncounterCount);
			int distinctVideos = grp.Select(x => x.VideoId.Value).Distinct().Count();
			if (distinctVideos >= 2 || totalEncounters >= 3)
			{
				highFrequencyUnsavedCount++;
			}
			else
			{
				justHeardOnceCount++;
			}
		}

		int masteredCount = 0;
		int activeInSrsCount = 0;

		foreach (var si in activeSavedItems)
		{
			var latestDetail = si.Schedule?.Details
				.OrderByDescending(d => d.ReviewNumber)
				.FirstOrDefault();

			bool isMastered = latestDetail is not null && (latestDetail.IntervalDays >= 21 || latestDetail.Repetitions >= 5);
			if (isMastered)
			{
				masteredCount++;
			}
			else
			{
				activeInSrsCount++;
			}
		}

		var exposureBuckets = new ExposureDepthBucketsDto(
			justHeardOnceCount,
			highFrequencyUnsavedCount,
			activeInSrsCount,
			masteredCount);

		// 3. Domain / Topic Coverage (Spec Case 4.2.3)
		var allEncounteredUnitIds = encounteredRows
			.Select(e => e.KnowledgeUnitId.Value)
			.ToHashSet();

		var domainCoverages = systemCategoryUnits
			.Select(cat =>
			{
				int totalInCat = cat.KnowledgeUnitIds.Count;
				int encounteredInCat = cat.KnowledgeUnitIds.Count(uid => allEncounteredUnitIds.Contains(uid));
				int pct = totalInCat > 0
					? (int)Math.Clamp(Math.Round((encounteredInCat * 100.0) / totalInCat), 0, 100)
					: 0;

				return new DomainTopicCoverageDto(
					cat.CategoryId,
					cat.CategoryName,
					encounteredInCat,
					totalInCat,
					pct);
			})
			.OrderByDescending(d => d.EncounteredUnitsCount)
			.ThenBy(d => d.CategoryName)
			.ToList();

		int totalSystemUnits = levelCoverages.Sum(l => l.TotalUnitsCount);
		int totalEncounteredUnits = allEncounteredUnitIds.Count;
		int overallPct = totalSystemUnits > 0
			? (int)Math.Clamp(Math.Round((totalEncounteredUnits * 100.0) / totalSystemUnits), 0, 100)
			: 0;

		return Result.Success(new VocabularyCoverageOverviewDto(
			targetLevelCode,
			totalEncounteredUnits,
			totalSystemUnits,
			overallPct,
			levelCoverages,
			exposureBuckets,
			domainCoverages));
	}
}
