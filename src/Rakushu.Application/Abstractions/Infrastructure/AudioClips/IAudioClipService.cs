namespace Rakushu.Application.Abstractions.Infrastructure.AudioClips;

public interface IAudioClipService
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
}
