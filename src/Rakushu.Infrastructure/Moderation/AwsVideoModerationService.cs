using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Moderation;
using Rakushu.Infrastructure.Extensions.Options;

namespace Rakushu.Infrastructure.Moderation;

internal sealed class AwsVideoModerationService(
	IAmazonTranscribeService transcribe,
	IAmazonRekognition rekognition,
	IOptions<AwsSettings> options) : IVideoModerationService
{
	private const float MinModerationConfidence = 75f;
	private static readonly List<string> LanguageOptions = ["ja-JP", "en-US", "vi-VN", "ko-KR", "zh-CN"];

	private readonly AwsSettings _settings = options.Value;

	public async Task<VideoModerationStatus> CheckAsync(
		Guid videoId,
		string storageKey,
		CancellationToken cancellationToken = default)
	{
		var language = await CheckLanguageAsync(videoId, storageKey, cancellationToken);
		var content = await CheckContentAsync(videoId, storageKey, cancellationToken);
		return new VideoModerationStatus(language, content);
	}

	private static string LanguageJobName(Guid videoId) => $"moderation-language-{videoId:N}";

	private async Task<LanguageModerationResult> CheckLanguageAsync(
		Guid videoId, string storageKey, CancellationToken ct)
	{
		var jobName = LanguageJobName(videoId);

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

		return status switch
		{
			"COMPLETED" => new LanguageModerationResult(
				ModerationJobState.Completed,
				job!.LanguageCode?.Value,
				job.IdentifiedLanguageScore,
				null),
			"FAILED" => new LanguageModerationResult(
				ModerationJobState.Failed, null, null, job!.FailureReason),
			_ => new LanguageModerationResult(ModerationJobState.InProgress, null, null, null)
		};
	}

	private async Task<ContentModerationResult> CheckContentAsync(
		Guid videoId, string storageKey, CancellationToken ct)
	{
		// ClientRequestToken makes StartContentModeration idempotent: the same video id always maps to the same job.
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
			ClientRequestToken = videoId.ToString("N"),
			JobTag = videoId.ToString("N"),
			MinConfidence = MinModerationConfidence
		}, ct);

		var labels = new List<string>();
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
				return new ContentModerationResult(ModerationJobState.InProgress, [], null);

			if (status == "FAILED")
				return new ContentModerationResult(ModerationJobState.Failed, [], response.StatusMessage);

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
			null);
	}
}
