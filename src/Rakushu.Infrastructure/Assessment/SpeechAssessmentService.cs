using Amazon.MediaConvert;
using Amazon.MediaConvert.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Assessment;
using Rakushu.Infrastructure.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rakushu.Infrastructure.Assessment;

public sealed class SpeechAssessmentService(
	HttpClient httpClient,
	IAmazonMediaConvert mediaConvert,
	IOptions<AwsSettings> options) : ISpeechAssessmentService
{
	private readonly AwsSettings _settings = options.Value;

	private const int RequiredSampleRate = 16000;
	private const int RequiredChannels = 1;
	private const int RequiredBitDepth = 16;

	private sealed record AudioInfo(
		string Container,
		string Codec,
		int SampleRate,
		int Channels,
		int? BitDepth,
		string ContentType,
		string Extension);

	public async Task<SpeechAssessmentResult> AssessAccuracyAsync(
		string referenceText,
		string userAudioUrl,
		CancellationToken cancellationToken)
	{
		// 1. Probe actual audio metadata.
		var audioInfo = await ProbeAudioAsync(
			userAudioUrl,
			cancellationToken);

		// 2. Validate whether Azure REST API supports this audio.
		ValidateAzureAudio(audioInfo);

		// 3. Download audio bytes.
		var audio = await httpClient.GetByteArrayAsync(
			userAudioUrl,
			cancellationToken);

		var config = JsonSerializer.Serialize(new
		{
			ReferenceText = referenceText,
			GradingSystem = "HundredMark",
			Granularity = "Phoneme",
			Dimension = "Comprehensive"
		});

		var url =
			$"https://{_settings.AzureSpeechRegion}.stt.speech.microsoft.com" +
			"/speech/recognition/conversation/cognitiveservices/v1" +
			"?language=ja-JP&format=detailed";

		using var request = new HttpRequestMessage(HttpMethod.Post, url);

		request.Headers.Add(
			"Ocp-Apim-Subscription-Key",
			_settings.AzureSpeechKey);

		request.Headers.Add(
			"Pronunciation-Assessment",
			Convert.ToBase64String(Encoding.UTF8.GetBytes(config)));

		request.Headers.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));

		request.Content = new ByteArrayContent(audio);

		// Azure documents this exact Content-Type.
		// Use TryAddWithoutValidation because MediaTypeHeaderValue.Parse
		// can reject the unquoted slash in codecs=audio/pcm.
		request.Content.Headers.TryAddWithoutValidation(
			"Content-Type",
			audioInfo.ContentType);

		using var response = await httpClient.SendAsync(
			request,
			cancellationToken);

		var raw = await response.Content.ReadAsStringAsync(
			cancellationToken);

		response.EnsureSuccessStatusCode();

		return new SpeechAssessmentResult(
			ParseAccuracy(raw),
			raw);
	}

	public async Task<SpeechAssessmentResult> AssessPitchSimilarityAsync(
		string sentence,
		string referenceAudioUrl,
		string userAudioUrl,
		CancellationToken cancellationToken)
	{
		// Probe both audio files to identify their actual formats.
		var referenceInfo = await ProbeAudioAsync(
			referenceAudioUrl,
			cancellationToken);

		var userInfo = await ProbeAudioAsync(
			userAudioUrl,
			cancellationToken);

		var reference = await httpClient.GetByteArrayAsync(
			referenceAudioUrl,
			cancellationToken);

		var user = await httpClient.GetByteArrayAsync(
			userAudioUrl,
			cancellationToken);

		using var referenceContent = new ByteArrayContent(reference);
		using var userContent = new ByteArrayContent(user);

		//referenceContent.Headers.ContentType =
		//	new MediaTypeHeaderValue(referenceInfo.ContentType.Split(';')[0]);

		//userContent.Headers.ContentType =
		//	new MediaTypeHeaderValue(userInfo.ContentType.Split(';')[0]);

		using var form = new MultipartFormDataContent();

		form.Add(
			referenceContent,
			"teacher_audio_file",
			$"reference.{referenceInfo.Extension}");

		form.Add(
			userContent,
			"student_audio_file",
			$"user.{userInfo.Extension}");

		form.Add(
	new StringContent(sentence),
	"sentence");

		form.Add(
	new StringContent("intensity"),
	"alignment_method");

		form.Add(
	new StringContent("true"),
	"fallback_if_no_alignment");

		var baseUrl = _settings.PitchAssessmentServiceBaseUrl.TrimEnd('/');

		using var response = await httpClient.PostAsync(
			$"{baseUrl}/compare",
			form,
			cancellationToken);

		var raw = await response.Content.ReadAsStringAsync(
			cancellationToken);

		response.EnsureSuccessStatusCode();

		using var doc = JsonDocument.Parse(raw);

		var scoreElement = doc.RootElement.GetProperty("score");

		decimal? score = scoreElement.ValueKind == JsonValueKind.Null
			? null
			: Math.Clamp(scoreElement.GetDecimal(), 0m, 100m);

		return new SpeechAssessmentResult(
			score,
			raw);
	}

	private async Task<AudioInfo> ProbeAudioAsync(
		string audioUrl,
		CancellationToken cancellationToken)
	{
		var response = await mediaConvert.ProbeAsync(
			new ProbeRequest
			{
				InputFiles =
				[
					new ProbeInputFile
					{
						FileUrl = audioUrl
					}
				]
			},
			cancellationToken);

		var container = response.ProbeResults?
			.FirstOrDefault()?
			.Container;

		if (container is null)
		{
			throw new InvalidOperationException(
				"Unable to probe audio container.");
		}

		var audioTrack = container.Tracks?
			.FirstOrDefault(t => t.TrackType == "audio");

		if (audioTrack is null)
		{
			throw new InvalidOperationException(
				"The provided media does not contain an audio track.");
		}

		var properties = audioTrack.AudioProperties;

		var containerFormat = container.Format ?? "unknown";
		var codec = audioTrack.Codec ?? "UNKNOWN";

		var sampleRate = properties?.SampleRate ?? 0;
		var channels = properties?.Channels ?? 0;
		var bitDepth = properties?.BitDepth;

		var contentType = ResolveContentType(
			containerFormat,
			codec,
			sampleRate,
			channels,
			bitDepth);

		var extension = ResolveExtension(containerFormat);

		return new AudioInfo(
			containerFormat,
			codec,
			sampleRate,
			channels,
			bitDepth,
			contentType,
			extension);
	}

	private static string ResolveContentType(
		string container,
		string codec,
		int sampleRate,
		int channels,
		int? bitDepth)
	{
		// Azure Speech REST API supports WAV PCM and OGG OPUS.
		if (container == "wave"
			&& codec == "PCM"
			&& sampleRate == RequiredSampleRate
			&& channels == RequiredChannels
			&& bitDepth == RequiredBitDepth)
		{
			return "audio/wav; codecs=audio/pcm; samplerate=16000";
		}

		if (container == "ogg"
			&& codec == "OPUS"
			&& sampleRate == RequiredSampleRate
			&& channels == RequiredChannels)
		{
			return "audio/ogg; codecs=opus";
		}

		return "unsupported";
	}

	private static void ValidateAzureAudio(AudioInfo audio)
	{
		if (audio.ContentType != "unsupported")
			return;

		throw new NotSupportedException(
			$"Azure Speech REST API does not support this audio format directly. " +
			$"Container: {audio.Container}, " +
			$"Codec: {audio.Codec}, " +
			$"SampleRate: {audio.SampleRate}, " +
			$"Channels: {audio.Channels}, " +
			$"BitDepth: {audio.BitDepth}. " +
			"Expected WAV/PCM/16kHz/mono/16-bit or OGG/OPUS/16kHz/mono. " +
			"Convert the audio before assessment.");
	}

	private static string ResolveExtension(string container)
	{
		return container switch
		{
			"wave" => "wav",
			"ogg" => "ogg",
			"webm" => "webm",
			"mp4" => "mp4",
			"matroska" => "mkv",
			_ => "bin"
		};
	}
	private static decimal ParseAccuracy(string raw)
	{
		using var doc = JsonDocument.Parse(raw);

		var segments = doc.RootElement.ValueKind == JsonValueKind.Array
			? doc.RootElement.EnumerateArray().ToList()
			: [doc.RootElement];

		var scores = new List<decimal>();

		foreach (var segment in segments)
		{
			if (segment.TryGetProperty("RecognitionStatus", out var status)
				&& status.GetString() != "Success")
			{
				continue;
			}

			if (!segment.TryGetProperty("NBest", out var nBest)
				|| nBest.ValueKind != JsonValueKind.Array
				|| nBest.GetArrayLength() == 0)
			{
				continue;
			}

			var bestHypothesis = nBest[0];

			if (bestHypothesis.TryGetProperty("AccuracyScore", out var accuracy)
				&& accuracy.ValueKind == JsonValueKind.Number)
			{
				scores.Add(accuracy.GetDecimal());
			}
		}

		return scores.Count == 0
			? 0m
			: Math.Round(scores.Average(), 2);
	}
}