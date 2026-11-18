namespace Rakushu.Application.Abstractions.Infrastructure.VideoImport;

public interface IVideoImportService
{
	// Download video from external source and upload it to storage using multipart upload
	Task<VideoImportResultDto> ImportAsync(
		string sourceUrl,
		CancellationToken cancellationToken);
}

public sealed record VideoImportResultDto(
	string StorageKey,
	string ContentType,
	string FileName,
	long FileSizeBytes);
