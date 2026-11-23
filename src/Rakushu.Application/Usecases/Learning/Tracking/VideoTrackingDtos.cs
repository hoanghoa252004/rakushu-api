namespace Rakushu.Application.Usecases.Learning.Tracking;

public sealed record WatchedRangeInput(
	double StartSeconds,
	double EndSeconds);

public sealed record EncounterSourceDto(
	Guid VideoId,
	string VideoTitle,
	double TimestampSeconds,
	string Sentence);

public sealed record SrsAdjustedItemDto(
	Guid SavedItemId,
	string Expression,
	string AdjustmentType,
	DateTimeOffset NewDueAt);

public sealed record RecordWatchHeartbeatResponseDto(
	Guid Id,
	int SrsAdjustedItemsCount,
	IReadOnlyList<SrsAdjustedItemDto> SrsAdjustedItems);

public sealed record VideoWatchProgressDto(
	Guid VideoId,
	double LastWatchedSecond,
	double TotalWatchedSeconds,
	double CompletionRate,
	bool IsCompleted,
	IReadOnlyList<double[]> WatchedIntervals,
	IReadOnlyList<Guid> EncounteredLearningUnitIds);

public sealed record WatchHistoryLearningUnitDto(
	Guid Id,
	Guid KnowledgeMeaningId,
	string Expression,
	string? Reading,
	string MeaningVi,
	string JlptLevel,
	string? Pos,
	double StartTime,
	double EndTime,
	string CueText,
	bool IsEncountered,
	int EncounterCount,
	int RawPlaybackCount,
	bool IsSavedToFlashcard,
	Guid? SavedItemId = null);

public sealed record WatchedVideoHistoryItemDto(
	Guid VideoId,
	string? Slug,
	string Title,
	string? JapaneseTitle,
	string? Thumbnail,
	double Duration,
	string DurationFormatted,
	Guid? SeriesId,
	string? SeriesTitle,
	string? CategoryName,
	DateTimeOffset LastWatchedAt,
	double LastPosition,
	IReadOnlyList<double[]> WatchedIntervals,
	double EffectiveWatchDuration,
	int WatchCoveragePercent,
	int TotalLearningUnitsCount,
	int EncounteredUnitsCount,
	int AbsorptionRatePercent,
	IReadOnlyList<WatchHistoryLearningUnitDto> LearningUnits);

public sealed record EncounteredVocabularySuggestionDto(
	Guid KnowledgeMeaningId,
	Guid KnowledgeUnitId,
	string Expression,
	string? Reading,
	string Meaning,
	string JlptLevel,
	string? CategoryName,
	bool MatchesUserInterest,
	bool MatchesTargetLevel,
	int TotalEncounterCount,
	int RawPlaybackCount,
	int DistinctVideoCount,
	string QualificationReason,
	DateTimeOffset LastEncounteredAt,
	IReadOnlyList<EncounterSourceDto> EncounterSources);

public sealed record KrashenVideoRecommendationDto(
	Guid VideoId,
	string Title,
	string? ThumbnailUrl,
	double DurationSeconds,
	string DurationFormatted,
	string? CategoryName,
	string? SeriesTitle,
	int TotalVocabularyCount,
	int KnownVocabularyCount,
	int NewVocabularyCount,
	double KnownRatio,
	int ComprehensibilityScore,
	string DifficultyLabel,
	string RecommendationReason);

public sealed record JlptLevelCoverageDto(
	string LevelCode,
	string LevelName,
	int SortOrder,
	int EncounteredUnitsCount,
	int TotalUnitsCount,
	int CoveragePercent);

public sealed record ExposureDepthBucketsDto(
	int JustHeardOnceCount,
	int HighFrequencyUnsavedCount,
	int ActiveInSrsCount,
	int MasteredCount);

public sealed record DomainTopicCoverageDto(
	Guid CategoryId,
	string CategoryName,
	int EncounteredUnitsCount,
	int TotalUnitsCount,
	int CoveragePercent);

public sealed record VocabularyCoverageOverviewDto(
	string? TargetLevelCode,
	int TotalEncounteredUniqueUnits,
	int TotalSystemUniqueUnits,
	int OverallCoveragePercent,
	IReadOnlyList<JlptLevelCoverageDto> LevelCoverages,
	ExposureDepthBucketsDto ExposureBuckets,
	IReadOnlyList<DomainTopicCoverageDto> DomainCoverages);
