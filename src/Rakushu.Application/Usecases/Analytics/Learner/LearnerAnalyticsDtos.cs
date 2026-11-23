using System.Text.Json.Serialization;

namespace Rakushu.Application.Usecases.Analytics.Learner;

// ============================================================================
// 1. LEARNER DASHBOARD (/api/analytics/learner/dashboard & /srs-heatmap)
// ============================================================================

public sealed record SrsMaturityBreakdownDto(
	int NewCount,
	int LearningCount,
	int YoungCount,
	int MasteredCount,
	int LeechCount);

public sealed record DailyReviewHeatmapPointDto(
	string Date,
	int ReviewCount,
	int SuccessCount);

public sealed record UpcomingDueForecastPointDto(
	string Date,
	int DueCount);

public sealed record SrsHeatmapResponseDto(
	int Year,
	IReadOnlyList<DailyReviewHeatmapPointDto> DailyReviewHeatmap);

public sealed record LearnerSrsAnalyticsDto(
	int CurrentStreakDays,
	int LongestStreakDays,
	SrsMaturityBreakdownDto MaturityBreakdown,
	IReadOnlyList<DailyReviewHeatmapPointDto> DailyReviewHeatmap,
	IReadOnlyList<UpcomingDueForecastPointDto> UpcomingDueForecast);

public sealed record ProficiencyLevelCoverageDto(
	string LevelCode,
	string LevelName,
	int SavedCount,
	int MasteredCount,
	double MasteryRate);

public sealed record KnowledgeTypeCoverageDto(
	string KnowledgeType,
	int SavedCount,
	int MasteredCount);

public sealed record KnowledgeMatrixCellDto(
	string LevelCode,
	string KnowledgeType,
	int SavedCount,
	int MasteredCount);

public sealed record LearnerKnowledgeCoverageDto(
	IReadOnlyList<ProficiencyLevelCoverageDto> ByProficiencyLevel,
	IReadOnlyList<KnowledgeTypeCoverageDto> ByKnowledgeType,
	IReadOnlyList<KnowledgeMatrixCellDto> Matrix);

public sealed record QuizModeSummaryDto(
	string Mode,
	int CompletedCount,
	double AverageScore);

public sealed record QuizSummaryDto(
	int TotalCompletedAttempts,
	int TotalAnsweredQuestions,
	double AverageScore,
	IReadOnlyList<QuizModeSummaryDto> ByMode);

public sealed record QuizScoreTrendPointDto(
	string Date,
	string Mode,
	int AttemptCount,
	double AverageScore);

public sealed record QuizWeakWordDto(
	Guid? SavedItemId,
	string Word,
	string? Reading,
	string Meaning,
	int WrongCount,
	int TotalTimesTested,
	double AvgTimeSeconds,
	string StatsTag);

public sealed record QuestionTypeDiagnosticDto(
	string QuestionTypeCode,
	string QuestionTypeName,
	int AnsweredCount,
	int CorrectCount,
	double AccuracyRate,
	double AvgTimeSpentSeconds,
	int ExpectedDurationSeconds,
	IReadOnlyList<QuizWeakWordDto> WeakWords);

public sealed record FlashcardRatingDistributionDto(
	[property: JsonPropertyName("Forgot")] int Forgot,
	[property: JsonPropertyName("Hard")] int Hard,
	[property: JsonPropertyName("Easy")] int Easy);

public sealed record FlashcardSummaryDto(
	int CompletedSessions,
	int TotalReviewedCards,
	FlashcardRatingDistributionDto RatingDistribution);

public sealed record LearnerPracticePerformanceDto(
	QuizSummaryDto QuizSummary,
	IReadOnlyList<QuizScoreTrendPointDto> QuizScoreTrend,
	IReadOnlyList<QuestionTypeDiagnosticDto> ByQuestionType,
	FlashcardSummaryDto FlashcardSummary);

public sealed record ShadowingWeakWordDto(
	string Word,
	string? Reading,
	string? Meaning,
	decimal? Score,
	string IssueType,
	string StatsTag);

public sealed record ShadowingScoreTrendPointDto(
	Guid AttemptId,
	Guid SessionId,
	DateTimeOffset CompletedAt,
	string MaterialName,
	string AssessmentType,
	decimal OverallScore,
	decimal AccuracyScore,
	decimal PitchSimilarityScore,
	IReadOnlyList<ShadowingWeakWordDto> WeakWords);

public sealed record LearnerShadowingProgressDto(
	int TotalAttempts,
	int AssessedAttempts,
	int CancelledAttempts,
	int TotalRecordedSeconds,
	int TotalLinesSubmitted,
	int SkippedLinesCount,
	double SkipRate,
	decimal? AverageOverallScore,
	decimal? AverageAccuracyScore,
	decimal? AveragePitchSimilarityScore,
	IReadOnlyList<ShadowingScoreTrendPointDto> ScoreTrend);

public sealed record VideoLearningSampleWordDto(
	string Expression,
	string? Reading,
	string Meaning);

public sealed record HarvestedVideoSummaryDto(
	Guid Id,
	string Title,
	int SavedWordsCount,
	IReadOnlyList<VideoLearningSampleWordDto> SampleWords);

public sealed record VideoTopicDistributionItemDto(
	Guid Id,
	string Name,
	int Count,
	int Percentage,
	string? ThemeColor,
	string? Slug = null);

public sealed record JlptDistributionItemDto(
	string Level,
	int Count,
	int Percentage);

public sealed record F1VideoLearningSummaryDto(
	int ExploredVideosCount,
	double AvgWordsPerVideo,
	int WeeklyHarvestCount,
	int WeeklyGrowthCount,
	int? WeeklyGrowthPercentage,
	DateTimeOffset? LastStudiedAt,
	int UploadedVideosCount,
	int UploadedTotalDurationSeconds,
	int AiReadyCount,
	int AiProcessingCount,
	int TotalSavedWords,
	int TotalTopicsCount,
	string TopCategoryName,
	int TopCategoryPercentage,
	IReadOnlyList<VideoTopicDistributionItemDto> TopicDistribution,
	IReadOnlyList<JlptDistributionItemDto> JlptDistribution,
	IReadOnlyList<HarvestedVideoSummaryDto> TopVideos);

public sealed record LearnerDashboardDto(
	int PeriodDays,
	DateTimeOffset GeneratedAt,
	F1VideoLearningSummaryDto VideoLearningSummary,
	LearnerSrsAnalyticsDto SrsAnalytics,
	LearnerKnowledgeCoverageDto KnowledgeCoverage,
	LearnerPracticePerformanceDto PracticePerformance,
	LearnerShadowingProgressDto ShadowingProgress);

// ============================================================================
// 3. QUIZ DIAGNOSTICS DETAIL (/api/analytics/learner/quiz-diagnostics)
// ============================================================================

public sealed record QuestionTypeSummaryDto(
	string Code,
	string Name,
	int ExpectedDurationSeconds,
	int TotalAnswered,
	int CorrectCount,
	double AccuracyRate,
	double AvgTimeSpentSeconds,
	bool IsWeakest);

public sealed record QuestionTypeWordEvidenceDto(
	Guid SavedItemId,
	string Expression,
	string? Reading,
	string Meaning,
	int TotalTimesTested,
	int CorrectCount,
	int WrongCount,
	double AvgTimeSeconds);

public sealed record QuestionTypeDetailResponseDto(
	IReadOnlyList<QuestionTypeSummaryDto> TypesOverview,
	IReadOnlyDictionary<string, IReadOnlyList<QuestionTypeWordEvidenceDto>> WordsByType,
	IReadOnlyList<QuestionTypeWordEvidenceDto> SelectedTypeWords);
