namespace Rakushu.Application.Usecases.Analytics.Curator;

public sealed record CuratorDashboardDto(
	int PeriodDays,
	string TimeZone,
	DateTimeOffset GeneratedAt,
	CuratorReportsAnalyticsDto Reports,
	CuratorVideosAnalyticsDto Videos,
	CuratorQuestionBankAnalyticsDto QuestionBank,
	CuratorKnowledgeAnalyticsDto Knowledge,
	CuratorLearnerActivityAnalyticsDto? LearnerActivity = null);

public sealed record CuratorSummaryDto(
	DateTimeOffset GeneratedAt,
	CuratorReportsSummaryCountsDto Reports,
	CuratorVideosSummaryCountsDto Videos,
	CuratorQuestionBankSummaryCountsDto QuestionBank,
	CuratorKnowledgeSummaryCountsDto Knowledge);

public sealed record CuratorReportsSummaryCountsDto(
	int TotalReports,
	int PendingCount,
	int InReviewCount,
	int ResolvedCount,
	int DismissedCount,
	int UnresolvedReportsCount = 0);

public sealed record CuratorVideosSummaryCountsDto(
	int TotalVideos,
	int ReadyCount,
	int ReviewingCount,
	int PendingCount,
	int ProcessingCount,
	int DraftCount,
	int RejectedCount,
	int ArchivedCount,
	int PendingReviewTotal = 0,
	int OtherStatusCount = 0,
	double ReadyRatePercent = 0.0);

public sealed record CuratorQuestionBankSummaryCountsDto(
	int TotalQuestions,
	int PublishedCount,
	int DraftCount,
	int InactiveCount,
	int MediaCount,
	int WithAudioCount,
	int WithVideoCount,
	int WithImageCount,
	int TextOnlyCount,
	double PublishedRatePercent = 0.0);

public sealed record CuratorKnowledgeSummaryCountsDto(
	int TotalCollections,
	int ActiveCollectionsCount,
	int InactiveCollectionsCount,
	int TotalKnowledgeUnits,
	int ActiveUnitsCount,
	int PendingUnitsCount,
	int OovDetectedUnitsCount,
	int PendingOovUnitsCount,
	int UnresolvedReportsCount = 0);

public sealed record CuratorReportsAnalyticsDto(
	int TotalReports,
	int PendingCount,
	int InReviewCount,
	int ResolvedCount,
	int DismissedCount,
	int PeriodNewCount,
	int PeriodResolvedCount,
	double? AvgResolutionHours,
	IReadOnlyList<CuratorReportDailyTrendPointDto> DailyTrend,
	IReadOnlyList<CuratorReportCategoryBreakdownDto> CategoryBreakdown,
	IReadOnlyList<CuratorReportTargetTypeBreakdownDto> TargetTypeBreakdown,
	IReadOnlyList<CuratorPriorityReportItemDto> PriorityReports,
	int UnresolvedReportsCount = 0);

public sealed record CuratorReportDailyTrendPointDto(
	string Date,
	string Label,
	int CreatedCount,
	int ResolvedCount);

public sealed record CuratorReportCategoryBreakdownDto(
	string Category,
	int Count);

public sealed record CuratorReportTargetTypeBreakdownDto(
	string TargetType,
	int Count);

public sealed record CuratorPriorityReportItemDto(
	Guid Id,
	string TargetType,
	Guid TargetId,
	string Category,
	string Status,
	int ReportCount,
	string? TargetPreview,
	string? LatestFeedback,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	double AgeHours);

public sealed record CuratorVideosAnalyticsDto(
	int TotalVideos,
	int ReadyCount,
	int ReviewingCount,
	int PendingCount,
	int ProcessingCount,
	int DraftCount,
	int RejectedCount,
	int ArchivedCount,
	int CuratedCount,
	int LearnerUploadedCount,
	int ExternalImportedCount,
	int PeriodAddedCount,
	int PeriodReadyCount,
	int UnreviewedTranscriptsCount,
	int UnreviewedLearningUnitsCount,
	int LowConfidenceLearningUnitsCount,
	IReadOnlyList<CuratorStatusBreakdownDto> StatusBreakdown,
	IReadOnlyList<CuratorVideoQueueItemDto> NeedsAttentionVideos,
	int PendingReviewTotal = 0,
	int OtherStatusCount = 0,
	double ReadyRatePercent = 0.0);

public sealed record CuratorStatusBreakdownDto(
	string Status,
	int Count);

public sealed record CuratorVideoQueueItemDto(
	Guid Id,
	string Title,
	string Status,
	string SourceType,
	string? ContentCategoryName,
	int DurationSeconds,
	int UnreviewedUnitsCount,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt);

public sealed record CuratorQuestionBankAnalyticsDto(
	int TotalQuestions,
	int PublishedCount,
	int DraftCount,
	int InactiveCount,
	int MediaCount,
	int WithAudioCount,
	int WithVideoCount,
	int WithImageCount,
	int TextOnlyCount,
	int PeriodCreatedCount,
	int PeriodReviewedCount,
	IReadOnlyList<CuratorQuestionTypeBreakdownDto> TypeDistribution,
	IReadOnlyList<CuratorDraftQuestionItemDto> RecentDraftQuestions,
	double PublishedRatePercent = 0.0);

public sealed record CuratorQuestionTypeBreakdownDto(
	Guid QuestionTypeId,
	string Code,
	string Name,
	int TotalCount,
	int PublishedCount,
	int DraftCount,
	double DraftPercent = 0.0);

public sealed record CuratorDraftQuestionItemDto(
	Guid Id,
	string QuestionTypeCode,
	string QuestionTypeName,
	string PromptText,
	string? KnowledgeUnitExpression,
	string MediaType,
	DateTimeOffset CreatedAt,
	string? JlptLevel = null);

public sealed record CuratorKnowledgeAnalyticsDto(
	int TotalCollections,
	int ActiveCollectionsCount,
	int InactiveCollectionsCount,
	int TotalKnowledgeUnits,
	int ActiveUnitsCount,
	int PendingUnitsCount,
	int OovDetectedUnitsCount,
	int PendingOovUnitsCount,
	int LowConfidenceLearningUnitsCount,
	int PeriodNewUnitsCount,
	IReadOnlyList<CuratorCollectionTypeBreakdownDto> CollectionTypeDistribution,
	IReadOnlyList<CuratorCollectionSummaryItemDto> FeaturedCollections);

public sealed record CuratorCollectionTypeBreakdownDto(
	string CollectionType,
	int TotalCount,
	int ActiveCount);

public sealed record CuratorCollectionSummaryItemDto(
	Guid Id,
	string Code,
	string Name,
	string CollectionType,
	string Scope,
	bool IsActive,
	int ItemsCount,
	DateTimeOffset UpdatedAt);

public sealed record CuratorLearnerActivityAnalyticsDto(
	int CompletedQuizAttempts,
	int TotalAnsweredItems,
	int TotalWrongItems,
	double AvgQuizAccuracyPercent,
	int ActiveQuizLearnersCount,
	int TotalHighErrorQuestionsCount,
	IReadOnlyList<CuratorLevelPerformanceDto> LevelBreakdown,
	IReadOnlyList<CuratorLevelErrorCountDto> LevelErrorCounts,
	IReadOnlyList<CuratorHighErrorQuestionItemDto> HighErrorQuestions);

public sealed record CuratorLevelPerformanceDto(
	string LevelCode,
	int AnsweredCount,
	int CorrectCount,
	double AccuracyPercent,
	int LearnersCount);

public sealed record CuratorHighErrorQuestionItemDto(
	Guid BankQuestionId,
	string QuestionTypeCode,
	string QuestionTypeName,
	string PromptText,
	string? KnowledgeUnitExpression,
	string? KnowledgeUnitReading,
	string JlptLevel,
	int TotalAnswers,
	int WrongAnswers,
	double ErrorRatePercent,
	int DistinctLearnersWrong);

public sealed record CuratorLevelErrorCountDto(
	string LevelCode,
	int QuestionCount);

public sealed record CuratorHighErrorQuestionsResponseDto(
	int PeriodDays,
	int TotalCount,
	int Page,
	int PageSize,
	int TotalPages,
	IReadOnlyList<CuratorLevelErrorCountDto> LevelCounts,
	IReadOnlyList<CuratorHighErrorQuestionItemDto> Items);
