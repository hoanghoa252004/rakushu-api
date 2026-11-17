namespace Rakushu.Application.Abstractions.Infrastructure.AudioClips;

public interface IMediaConvertService
{
	/// <summary>
	/// Extracts the [start, end] audio range of the source media into a new clip and returns its storage key.
	/// </summary>
	Task<string> ExtractClipAsync(
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken);

	/// <summary>Creates a clip-extraction job tagged with roleLineId user metadata, without waiting for it.</summary>
	Task CreateClipJobAsync(
		Guid roleLineId,
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken);

	/// <summary>Lists clip jobs (all statuses) whose user metadata roleLineId is in the given set.</summary>
	Task<IReadOnlyList<NormalizeJobInfo>> ListClipJobsAsync(
		IReadOnlyCollection<Guid> roleLineIds,
		CancellationToken cancellationToken);

	Task<string> NormalizeForSpeechAssessmentAsync(
	string sourceKey,
	string outputKeyWithoutExtension,
	CancellationToken cancellationToken);

	/// <summary>Creates a normalize-to-WAV job tagged with attemptId/roleLineId user metadata, without waiting for it.</summary>
	Task CreateNormalizeJobAsync(
		Guid attemptId,
		Guid roleLineId,
		string sourceKey,
		string outputKeyWithoutExtension,
		CancellationToken cancellationToken);

	/// <summary>Lists normalize jobs (by status) whose user metadata attemptId matches.</summary>
	Task<IReadOnlyList<NormalizeJobInfo>> ListNormalizeJobsAsync(
		Guid attemptId,
		CancellationToken cancellationToken);
}

public enum NormalizeJobStatus
{
	InProgress,
	Complete,
	Failed
}

public sealed record NormalizeJobInfo(Guid RoleLineId, NormalizeJobStatus Status, string? Error);
