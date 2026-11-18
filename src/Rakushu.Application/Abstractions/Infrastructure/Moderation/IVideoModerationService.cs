namespace Rakushu.Application.Abstractions.Infrastructure.Moderation;

public interface IVideoModerationService
{
	/// <summary>
	/// Starts (idempotently, keyed by video id) the language identification and content moderation jobs
	/// and returns their current state.
	/// </summary>
	Task<VideoModerationStatus> CheckAsync(
		Guid videoId,
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
	string? FailureReason);

public sealed record ContentModerationResult(
	ModerationJobState State,
	IReadOnlyList<string> FlaggedLabels,
	string? FailureReason);

public sealed record VideoModerationStatus(
	LanguageModerationResult Language,
	ContentModerationResult Content)
{
	public bool IsCompleted =>
		Language.State != ModerationJobState.InProgress &&
		Content.State != ModerationJobState.InProgress;
}
