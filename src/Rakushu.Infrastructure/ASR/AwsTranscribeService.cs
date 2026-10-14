using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.ASR;

internal sealed class AwsTranscribeService : IAsrService
{
	private readonly IAmazonTranscribeService _client;
	private readonly AwsSettings _awsSettings;

	public AwsTranscribeService(
		 IOptions<AwsSettings> options,
		IAmazonTranscribeService client)
	{
		_client = client;
		_awsSettings = options.Value;
	}

	public async Task<StartTranscriptionResult> StartAsync(
		StartTranscriptionRequest request,
		CancellationToken cancellationToken = default)
	{
		var awsRequest = new StartTranscriptionJobRequest
		{
			TranscriptionJobName = request.JobName,

			Media = new Media
			{
				MediaFileUri = request.MediaUri
			},

			MediaFormat = ParseMediaFormat(request.MediaFormat),

			LanguageCode = ParseLanguageCode(request.LanguageCode),

			OutputBucketName = _awsSettings.BucketName,

			OutputKey = $"transcripts/{request.JobName}-transcript.json",

			Settings = new Settings
			{
				ShowSpeakerLabels = request.EnableSpeakerIdentification,

				MaxSpeakerLabels = request.EnableSpeakerIdentification
					? request.MaxSpeakerLabels
					: null
			}
		};

		var response = await _client.StartTranscriptionJobAsync(
			awsRequest,
			cancellationToken);

		var job = response.TranscriptionJob
			?? throw new InvalidOperationException(
				"AWS did not return a transcription job.");

		return new StartTranscriptionResult(job.TranscriptionJobName, job.TranscriptionJobStatus.Value);
	}

	public async Task<TranscriptionJobResult?> GetJobAsync(string jobName, CancellationToken cancellationToken = default)
	{
		try
		{
			var response = await _client.GetTranscriptionJobAsync(
				new GetTranscriptionJobRequest
				{
					TranscriptionJobName = jobName
				},
				cancellationToken);

			var job = response.TranscriptionJob;

			if (job is null)
			{
				return null;
			}

			return new TranscriptionJobResult(
				job.TranscriptionJobName,
				job.TranscriptionJobStatus.Value,
				job.Transcript?.TranscriptFileUri,
				job.FailureReason);
		}
		catch (NotFoundException)
		{
			return null;
		}
	}

	public async Task<bool> CancelAsync(string jobName, CancellationToken cancellationToken = default)
	{
		await _client.DeleteTranscriptionJobAsync(
			new DeleteTranscriptionJobRequest
			{
				TranscriptionJobName = jobName
			},
			cancellationToken);

		return true;
	}

	private static MediaFormat ParseMediaFormat(string format)
	{
		return format.ToLowerInvariant() switch
		{
			"mp3" => MediaFormat.Mp3,
			"mp4" => MediaFormat.Mp4,
			"wav" => MediaFormat.Wav,
			"flac" => MediaFormat.Flac,
			"ogg" => MediaFormat.Ogg,
			"webm" => MediaFormat.Webm,
			"amr" => MediaFormat.Amr,
			_ => throw new ArgumentException(
				$"Unsupported media format: {format}",
				nameof(format))
		};
	}

	private static LanguageCode ParseLanguageCode(string languageCode)
	{
		return languageCode switch
		{
			"ja-JP" => LanguageCode.JaJP,
			"en-US" => LanguageCode.EnUS,
			"vi-VN" => LanguageCode.ViVN,
			_ => throw new ArgumentException(
				$"Unsupported language code: {languageCode}",
				nameof(languageCode))
		};
	}
}