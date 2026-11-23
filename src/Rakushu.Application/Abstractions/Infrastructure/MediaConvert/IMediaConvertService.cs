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

	/// <summary>Creates a clip-extraction job tagged with jobLogId user metadata, without waiting for it. Returns the MediaConvert job id.</summary>
	Task<string> CreateClipJobAsync(
		Guid jobLogId,
		string sourceKey,
		string outputKeyWithoutExtension,
		TimeSpan start,
		TimeSpan end,
		CancellationToken cancellationToken);

	/// <summary>Lists clip jobs (all statuses) whose user metadata jobLogId is in the given set.</summary>
	Task<IReadOnlyList<NormalizeJobInfo>> ListClipJobsAsync(
		IReadOnlyCollection<Guid> jobLogIds,
		CancellationToken cancellationToken);

	Task<string> NormalizeForSpeechAssessmentAsync(
	string sourceKey,
	string outputKeyWithoutExtension,
	CancellationToken cancellationToken);

	/// <summary>Creates a normalize-to-WAV job tagged with jobLogId user metadata, without waiting for it. Returns the MediaConvert job id.</summary>
	Task<string> CreateNormalizeJobAsync(
		Guid jobLogId,
		string sourceKey,
		string outputKeyWithoutExtension,
		CancellationToken cancellationToken);

	/// <summary>Lists normalize jobs (by status) whose user metadata jobLogId is in the given set.</summary>
	Task<IReadOnlyList<NormalizeJobInfo>> ListNormalizeJobsAsync(
		IReadOnlyCollection<Guid> jobLogIds,
		CancellationToken cancellationToken);
}

public enum NormalizeJobStatus
{
	InProgress,
	Complete,
	Failed
}

public sealed record NormalizeJobInfo(Guid JobLogId, NormalizeJobStatus Status, string? Error, string? JobId = null, string? RawResponse = null);
