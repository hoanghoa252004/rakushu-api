using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Application.Abstractions.Infrastructure.Moderation;
using Rakushu.Infrastructure.Extensions.Options;

namespace Rakushu.Infrastructure.Moderation;

internal sealed class AwsVideoModerationService(
	IAmazonTranscribeService transcribe,
	IAmazonRekognition rekognition,
	AwsRuntimeSettings options) : IVideoModerationService
{
	private const float MinModerationConfidence = 75f;
	private static readonly List<string> LanguageOptions = ["ja-JP", "en-US", "vi-VN", "ko-KR", "zh-CN"];

	private readonly AwsRuntimeSettings _settings = options;

	private static string LanguageJobName(Guid jobLogId) => $"moderation-language-{jobLogId:N}";

	public async Task<LanguageModerationResult> CheckLanguageAsync(
		Guid jobLogId, string storageKey, CancellationToken ct = default)
	{
		var jobName = LanguageJobName(jobLogId);

		TranscriptionJob? job = null;
		try
		{
			job = (await transcribe.GetTranscriptionJobAsync(
				new GetTranscriptionJobRequest { TranscriptionJobName = jobName }, ct)).TranscriptionJob;
		}
		catch (Exception ex) when (ex is NotFoundException or BadRequestException)
		{
		}

		if (job is null)
		{
			var started = await transcribe.StartTranscriptionJobAsync(new StartTranscriptionJobRequest
			{
				TranscriptionJobName = jobName,
				Media = new Media { MediaFileUri = $"s3://{_settings.BucketName}/{storageKey}" },
				IdentifyLanguage = true,
				LanguageOptions = LanguageOptions
			}, ct);

			job = started.TranscriptionJob;
		}

		var status = job?.TranscriptionJobStatus?.Value;
		var raw = JobLogSerializer.Serialize(job);

		return status switch
		{
			"COMPLETED" => new LanguageModerationResult(
				ModerationJobState.Completed,
				job!.LanguageCode?.Value,
				job.IdentifiedLanguageScore,
				null,
				jobName,
				raw),
			"FAILED" => new LanguageModerationResult(
				ModerationJobState.Failed, null, null, job!.FailureReason, jobName, raw),
			_ => new LanguageModerationResult(ModerationJobState.InProgress, null, null, null, jobName)
		};
	}

	public async Task<ContentModerationResult> CheckContentAsync(
		Guid jobLogId, string storageKey, CancellationToken ct = default)
	{
		// ClientRequestToken makes StartContentModeration idempotent: the same job log id always maps to the same job.
		var started = await rekognition.StartContentModerationAsync(new StartContentModerationRequest
		{
			Video = new Amazon.Rekognition.Model.Video
			{
				S3Object = new Amazon.Rekognition.Model.S3Object
				{
					Bucket = _settings.BucketName,
					Name = storageKey
				}
			},
			ClientRequestToken = jobLogId.ToString("N"),
			JobTag = jobLogId.ToString("N"),
			MinConfidence = MinModerationConfidence
		}, ct);

		var labels = new List<string>();
		var responses = new List<GetContentModerationResponse>();
		string? nextToken = null;

		do
		{
			var response = await rekognition.GetContentModerationAsync(new GetContentModerationRequest
			{
				JobId = started.JobId,
				MaxResults = 1000,
				NextToken = nextToken
			}, ct);

			var status = response.JobStatus?.Value;

			if (status == "IN_PROGRESS")
				return new ContentModerationResult(ModerationJobState.InProgress, [], null, started.JobId);

			if (status == "FAILED")
				return new ContentModerationResult(
					ModerationJobState.Failed, [], response.StatusMessage, started.JobId, JobLogSerializer.Serialize(response));

			responses.Add(response);

			labels.AddRange(response.ModerationLabels
				.Select(m => m.ModerationLabel)
				.Where(l => l is not null && l.Confidence >= MinModerationConfidence)
				.Select(l => l.Name));

			nextToken = labels.Count > 0 ? null : response.NextToken;
		}
		while (!string.IsNullOrEmpty(nextToken));

		return new ContentModerationResult(
			ModerationJobState.Completed,
			labels.Distinct().ToList(),
			null,
			started.JobId,
			JobLogSerializer.Serialize(responses));
	}
}
