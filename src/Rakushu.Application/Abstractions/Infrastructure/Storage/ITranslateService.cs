namespace Rakushu.Application.Abstractions.Infrastructure.Translation;

public interface ITranslateService
{
	Task<string> StartBatchTranslationJobAsync(
		string inputS3Uri,
		string outputS3Uri,
		string sourceLanguageCode,
		string targetLanguageCode,
		string jobName,
		CancellationToken cancellationToken);

	Task<TranslationJobStatusDto> GetBatchTranslationJobAsync(
		string jobId,
		CancellationToken cancellationToken);

	Task<TranslationJobStatusDto?> FindBatchTranslationJobByNameAsync(
		string jobName,
		CancellationToken cancellationToken);
}


public sealed record TranslationJobStatusDto(
	string JobId,
	string JobName,
	string Status,
	string? OutputS3Uri,
	string? Message);