using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;

public sealed record ShadowingAttemptLineContextModel(
	Guid AttemptDetailId,
	Guid RoleLineId,
	int SortOrder,
	string LineText,
	string? VietnameseMeaning,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string? UserAudioKey,
	string? ReferenceAudioKey,
	long? DurationMs,
	bool IsSkipped,
	DateTimeOffset? SubmittedAt);

public sealed record ShadowingAssessmentDetailContextModel(
	Guid AttemptDetailId,
	decimal? AccuracyScore,
	decimal? PitchSimilarityScore,
	string? AccuracyAssessmentResponse,
	string? Feedback);

public sealed record ShadowingAssessmentContextModel(
	Guid AssessmentId,
	ShadowingAssessmentType Type,
	decimal? OverallScore,
	string? OverallFeedback,
	DateTimeOffset CreatedAt,
	IReadOnlyList<ShadowingAssessmentDetailContextModel> Details);

public sealed record ShadowingAttemptAssessmentContextModel(
	Guid AttemptId,
	Guid SessionId,
	Guid MemberId,
	string MaterialName,
	string? MaterialJapaneseName,
	string? VideoTitle,
	string RoleName,
	ShadowingAttemptStatus AttemptStatus,
	DateTimeOffset StartedAt,
	DateTimeOffset? CompletedAt,
	IReadOnlyList<ShadowingAttemptLineContextModel> Lines,
	IReadOnlyList<ShadowingAssessmentContextModel> Assessments,
	string? SessionTitle = null,
	string? LearnerName = null);

public sealed record ShadowingAttemptLineAssessmentDto(
	Guid AttemptDetailId,
	Guid RoleLineId,
	int SortOrder,
	string LineText,
	string? VietnameseMeaning,
	TimeSpan StartTime,
	TimeSpan EndTime,
	long? DurationMs,
	bool IsSkipped,
	DateTimeOffset? SubmittedAt,
	string? UserAudioUrl,
	string? ReferenceAudioUrl,
	decimal? AccuracyScore,
	decimal? PitchSimilarityScore,
	string? Feedback,
	IReadOnlyList<ShadowingWordBreakdownDto> Words);

public sealed record ShadowingAttemptAssessmentItemDto(
	Guid AssessmentId,
	string AssessmentType,
	decimal? OverallScore,
	decimal? AverageAccuracyScore,
	decimal? AveragePitchSimilarityScore,
	string? OverallFeedback,
	DateTimeOffset CreatedAt,
	IReadOnlyList<ShadowingAttemptLineAssessmentDto> Lines);

public sealed record ShadowingAttemptAssessmentsResponseDto(
	Guid AttemptId,
	Guid SessionId,
	string MaterialName,
	string? MaterialJapaneseName,
	string? VideoTitle,
	string RoleName,
	string AttemptStatus,
	DateTimeOffset StartedAt,
	DateTimeOffset? CompletedAt,
	int TotalRecordedSeconds,
	int CompletedLinesCount,
	int SkippedLinesCount,
	ShadowingAttemptAssessmentItemDto? PrimaryAssessment,
	IReadOnlyList<ShadowingAttemptAssessmentItemDto> Assessments,
	IReadOnlyList<ShadowingAttemptLineAssessmentDto> Lines,
	string? ReevaluationRequestStatus = null,
	Guid? ReevaluationIssueId = null,
	Guid? ReevaluationReportId = null);
