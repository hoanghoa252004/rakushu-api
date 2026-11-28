namespace Rakushu.Application.Usecases.Analytics.Shadowing;

public sealed record ShadowingAssessmentLineInput(
	string LineText,
	string? VietnameseMeaning,
	decimal? AccuracyScore,
	string? AccuracyAssessmentResponse,
	decimal? PitchSimilarityScore,
	string? Feedback);

public sealed record ShadowingWordBreakdownDto(
	string Text,
	decimal Score,
	string Status,
	string? ErrorType,
	string? Tag);

public sealed record ScoreGradeDistributionDto(
	int Pass,
	int Tier60To79,
	int Tier30To59,
	int Under30);

public sealed record ShadowingHistoryItemDto(
	Guid AttemptId,
	Guid SessionId,
	string MaterialName,
	string? MaterialJapaneseName,
	string RoleName,
	DateTimeOffset CompletedAt,
	int DurationSeconds,
	decimal AccuracyScore,
	decimal PitchScore,
	decimal OverallScore,
	string AssessmentType,
	string AssessmentTier = "NEEDS_WORK");

public sealed record ShadowingDetailScoreTrendPointDto(
	string Date,
	DateTimeOffset CompletedAt,
	decimal OverallScore,
	decimal AccuracyScore,
	decimal PitchScore,
	string MaterialName,
	string? MaterialJapaneseName,
	Guid AttemptId,
	Guid SessionId);

public sealed record ShadowingAnalyticsDetailResponseDto(
	int TotalAttempts,
	int CompletedAttempts,
	int TotalRecordedSeconds,
	int CompletedLinesCount,
	int SkippedLinesCount,
	double SkipRatePercentage,
	decimal AverageOverallScore,
	IReadOnlyList<ShadowingDetailScoreTrendPointDto> ScoreTrend,
	IReadOnlyList<ShadowingHistoryItemDto> History,
	ScoreGradeDistributionDto? GradeDistribution = null);
