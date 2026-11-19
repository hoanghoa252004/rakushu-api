namespace Rakushu.Application.Usecases.Curation;

public sealed record SubmitIssueResponse(Guid Id);

public sealed record CurationTargetSnapshotDto(
	string TargetType,
	string? QuestionTitle = null,
	string? QuestionTypeCode = null,
	string? AnswersJson = null,
	string? CorrectAnswer = null,
	string? Explanation = null,
	Guid? VideoId = null,
	string? VideoTitle = null,
	double? StartTimeSeconds = null,
	double? EndTimeSeconds = null,
	string? OriginalSentenceJp = null,
	string? SubtitleVn = null,
	string? TargetToken = null,
	int? StartTokenIndex = null,
	int? EndTokenIndex = null,
	string? Expression = null,
	string? Reading = null,
	string? Meaning = null);

public sealed record LearnerTargetContextDto(
	string? QuestionTitle,
	string? QuestionTypeCode,
	string? AnswersJson,
	string? CorrectAnswer,
	string? Explanation,
	Guid? VideoId,
	string? VideoTitle,
	string? VideoUrl,
	string? VideoTimestampFormatted,
	double? StartTimeSeconds,
	double? EndTimeSeconds,
	string? OriginalSentenceJp,
	string? SubtitleVn,
	string? TargetToken,
	int? StartTokenIndex,
	int? EndTokenIndex,
	string? Expression,
	string? Reading,
	string? Meaning,
	bool? IsActive = null);

public sealed record LearnerIssueHistoryDto(
	Guid Id,
	string TargetType,
	Guid TargetId,
	string? TargetTitle,
	string Category,
	string? LearnerFeedback,
	string ReportStatus,
	string? CuratorNotes,
	DateTimeOffset CreatedAt,
	DateTimeOffset? ResolvedAt,
	LearnerTargetContextDto? TargetContext,
	CurationTargetSnapshotDto? ReportedSnapshot = null);

public sealed record CuratorReportSummaryDto(
	Guid ReportId,
	string TargetType,
	Guid TargetId,
	string? TargetTitle,
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
	IReadOnlyList<IssueFeedbackDto> Feedbacks,
	ReportTargetContextDto? Context,
	CurationTargetSnapshotDto? ReportedSnapshot = null);

public sealed record IssueFeedbackDto(
	Guid UserId,
	string? UserName,
	string? LearnerFeedback,
	DateTimeOffset CreatedAt);

public sealed record ReportTargetContextDto(
	BankQuestionContextDto? QuestionContext,
	VideoLearningUnitContextDto? VideoContext);

public sealed record BankQuestionContextDto(
	Guid QuestionId,
	string QuestionTitle,
	string QuestionTypeCode,
	string AnswersJson,
	string CorrectAnswer,
	string? Explanation,
	string? TargetExpression,
	string? TargetReading,
	string? TargetMeaning,
	bool IsActive = true);

public sealed record VideoLearningUnitContextDto(
	Guid? LearningUnitId,
	Guid VideoId,
	string VideoTitle,
	string? VideoUrl,
	string? VideoThumbnail,
	double VideoDurationSeconds,
	string VideoDurationFormatted,
	double StartTimeSeconds,
	double EndTimeSeconds,
	double TimestampSeconds,
	string TimestampFormatted,
	string OriginalSentenceJp,
	string? SubtitleVn,
	string? TargetToken,
	int StartTokenIndex,
	int EndTokenIndex,
	string? Expression,
	string? Meaning);
