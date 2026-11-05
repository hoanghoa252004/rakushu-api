namespace Rakushu.Application.Usecases.Curation;

public sealed record SubmitIssueResponse(
	Guid IssueDetailId,
	string Status,
	string Message);

public sealed record LearnerIssueHistoryDto(
	Guid Id,
	string TargetType,
	Guid TargetId,
	string Category,
	string? LearnerFeedback,
	string ReportStatus,
	DateTimeOffset CreatedAt);

public sealed record CuratorReportSummaryDto(
	Guid ReportId,
	string TargetType,
	Guid TargetId,
	string Category,
	string Status,
	int ReportCount,
	DateTimeOffset CreatedAt,
	DateTimeOffset? ResolvedAt);

public sealed record CuratorReportDetailDto(
	Guid ReportId,
	string TargetType,
	Guid TargetId,
	string Category,
	string Status,
	string? CuratorNotes,
	DateTimeOffset CreatedAt,
	DateTimeOffset? ResolvedAt,
	IReadOnlyList<IssueFeedbackDto> Feedbacks);

public sealed record IssueFeedbackDto(
	Guid UserId,
	string? UserName,
	string? LearnerFeedback,
	DateTimeOffset CreatedAt);
