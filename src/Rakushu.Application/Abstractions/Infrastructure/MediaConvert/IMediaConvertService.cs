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

	Task<string> NormalizeForSpeechAssessmentAsync(
	string sourceKey,
	string outputKeyWithoutExtension,
	CancellationToken cancellationToken);
}
