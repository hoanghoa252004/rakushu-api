using Amazon.TranscribeService.Model;
using Amazon.Translate;
using Amazon.Translate.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Infrastructure.Extensions.Options;

namespace Rakushu.Infrastructure.Translate;

internal sealed class AwsTranslateService : ITranslateService
{
	private readonly IAmazonTranslate _translate;
	private readonly AwsSettings _awsSettings;

	public AwsTranslateService(
		IAmazonTranslate translate,
		IOptions<AwsSettings> options)
	{
		_translate = translate;
		_awsSettings = options.Value;
	}

	public async Task<string> StartBatchTranslationJobAsync(
		string inputS3Uri,
		string outputS3Uri,
		string sourceLanguageCode,
		string targetLanguageCode,
		string jobName,
		CancellationToken cancellationToken)
	{
		var request = new StartTextTranslationJobRequest
		{
			JobName = jobName,

			InputDataConfig = new Amazon.Translate.Model.InputDataConfig
			{
				S3Uri = inputS3Uri,
				ContentType = "text/plain"
			},

			OutputDataConfig = new OutputDataConfig
			{
				S3Uri = outputS3Uri
			},

			SourceLanguageCode = sourceLanguageCode,
			TargetLanguageCodes = new List<string>
			{
				targetLanguageCode
			},

			DataAccessRoleArn = _awsSettings.TranslateDataAccessRoleArn
		};

		var response = await _translate.StartTextTranslationJobAsync(
			request,
			cancellationToken);

		return response.JobId;
	}


	public async Task<TranslationJobStatusDto?> FindBatchTranslationJobByNameAsync(
		string jobName,
		CancellationToken cancellationToken)
	{
		string? nextToken = null;

		do
		{
			var response = await _translate.ListTextTranslationJobsAsync(
				new ListTextTranslationJobsRequest
				{
					Filter = new TextTranslationJobFilter
					{
						JobName = jobName
					},
					NextToken = nextToken,
					MaxResults = 100
				},
				cancellationToken);

			var job = response.TextTranslationJobPropertiesList
				.FirstOrDefault(x => x.JobName == jobName);

			if (job is not null)
			{
				return MapJob(job);
			}

			nextToken = response.NextToken;
		}
		while (!string.IsNullOrWhiteSpace(nextToken));

		return null;
	}

	public async Task<TranslationJobStatusDto> GetBatchTranslationJobAsync(
		string jobId,
		CancellationToken cancellationToken)
	{
		var response = await _translate.DescribeTextTranslationJobAsync(
			new DescribeTextTranslationJobRequest
			{
				JobId = jobId
			},
			cancellationToken);

		return MapJob(response.TextTranslationJobProperties);
	}

	private static TranslationJobStatusDto MapJob(
		TextTranslationJobProperties job)
	{
		return new TranslationJobStatusDto(
			JobId: job.JobId,
			JobName: job.JobName,
			Status: job.JobStatus.Value,
			OutputS3Uri: job.OutputDataConfig?.S3Uri,
			Message: job.Message);
	}
}