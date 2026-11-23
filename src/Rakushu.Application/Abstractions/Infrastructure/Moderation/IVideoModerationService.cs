namespace Rakushu.Application.Abstractions.Infrastructure.Moderation;

public interface IVideoModerationService
{
	/// <summary>
	/// Starts (idempotently, keyed by the job log id) the language identification job and returns its current state.
	/// </summary>
	Task<LanguageModerationResult> CheckLanguageAsync(
		Guid jobLogId,
		string storageKey,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Starts (idempotently, keyed by the job log id) the content moderation job and returns its current state.
	/// </summary>
	Task<ContentModerationResult> CheckContentAsync(
		Guid jobLogId,
		string storageKey,
		CancellationToken cancellationToken = default);
}

public enum ModerationJobState
{
	InProgress = 1,
	Completed = 2,
	Failed = 3
}

public sealed record LanguageModerationResult(
	ModerationJobState State,
	string? LanguageCode,
	float? Score,
	string? FailureReason,
	string? JobLabel = null,
	string? RawResponse = null);

public sealed record ContentModerationResult(
	ModerationJobState State,
	IReadOnlyList<string> FlaggedLabels,
	string? FailureReason,
	string? JobLabel = null,
	string? RawResponse = null);

public sealed record VideoModerationStatus(
	LanguageModerationResult Language,
	ContentModerationResult Content)
{
	public bool IsCompleted =>
		Language.State != ModerationJobState.InProgress &&
		Content.State != ModerationJobState.InProgress;
}
