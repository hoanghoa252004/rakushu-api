namespace Rakushu.Application.Usecases.Analytics.Curator;

public sealed record CuratorDashboardDto(
	int PeriodDays,
	string TimeZone,
	DateTimeOffset GeneratedAt,
	CuratorReportsAnalyticsDto Reports,
	CuratorVideosAnalyticsDto Videos,
	CuratorQuestionBankAnalyticsDto QuestionBank,
	CuratorKnowledgeAnalyticsDto Knowledge);

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
	int DismissedCount);

public sealed record CuratorVideosSummaryCountsDto(
	int TotalVideos,
	int ReadyCount,
	int ReviewingCount,
	int PendingCount,
	int ProcessingCount,
	int DraftCount,
	int RejectedCount,
	int ArchivedCount);

public sealed record CuratorQuestionBankSummaryCountsDto(
	int TotalQuestions,
	int PublishedCount,
	int DraftCount,
	int InactiveCount,
	int MediaCount,
	int WithAudioCount,
	int WithVideoCount,
	int WithImageCount,
	int TextOnlyCount);

public sealed record CuratorKnowledgeSummaryCountsDto(
	int TotalCollections,
	int ActiveCollectionsCount,
	int InactiveCollectionsCount,
	int TotalKnowledgeUnits,
	int ActiveUnitsCount,
	int PendingUnitsCount,
	int OovDetectedUnitsCount,
	int PendingOovUnitsCount);

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
	IReadOnlyList<CuratorPriorityReportItemDto> PriorityReports);

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
	IReadOnlyList<CuratorVideoQueueItemDto> NeedsAttentionVideos);

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
	IReadOnlyList<CuratorDraftQuestionItemDto> RecentDraftQuestions);

public sealed record CuratorQuestionTypeBreakdownDto(
	Guid QuestionTypeId,
	string Code,
	string Name,
	int TotalCount,
	int PublishedCount,
	int DraftCount);

public sealed record CuratorDraftQuestionItemDto(
	Guid Id,
	string QuestionTypeCode,
	string QuestionTypeName,
	string PromptText,
	string? KnowledgeUnitExpression,
	string MediaType,
	DateTimeOffset CreatedAt);

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
