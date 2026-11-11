using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Assessment;
using Rakushu.Infrastructure.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rakushu.Infrastructure.Assessment;

public sealed class SpeechAssessmentService(
	HttpClient httpClient,
	IOptions<AwsSettings> options) : ISpeechAssessmentService
{
	private readonly AwsSettings _settings = options.Value;

	public async Task<SpeechAssessmentResult> AssessAccuracyAsync(
		string referenceText, string userAudioUrl, CancellationToken cancellationToken)
	{
		var audio = await httpClient.GetByteArrayAsync(userAudioUrl, cancellationToken);

		var config = JsonSerializer.Serialize(new
		{
			ReferenceText = referenceText,
			GradingSystem = "HundredMark",
			Granularity = "Phoneme",
			Dimension = "Comprehensive"
		});

		var url = $"https://{_settings.AzureSpeechRegion}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=ja-JP&format=detailed";
		using var request = new HttpRequestMessage(HttpMethod.Post, url);
		request.Headers.Add("Ocp-Apim-Subscription-Key", _settings.AzureSpeechKey);
		request.Headers.Add("Pronunciation-Assessment", Convert.ToBase64String(Encoding.UTF8.GetBytes(config)));
		request.Content = new ByteArrayContent(audio);
		request.Content.Headers.ContentType =
			MediaTypeHeaderValue.Parse("audio/wav; codecs=audio/pcm; samplerate=16000");

		using var response = await httpClient.SendAsync(request, cancellationToken);
		var raw = await response.Content.ReadAsStringAsync(cancellationToken);
		response.EnsureSuccessStatusCode();

		return new SpeechAssessmentResult(ParseAccuracy(raw), raw);
	}

	public async Task<SpeechAssessmentResult> AssessPitchSimilarityAsync(
		string referenceAudioUrl, string userAudioUrl, CancellationToken cancellationToken)
	{
		var reference = await httpClient.GetByteArrayAsync(referenceAudioUrl, cancellationToken);
		var user = await httpClient.GetByteArrayAsync(userAudioUrl, cancellationToken);

		using var form = new MultipartFormDataContent
		{
			{ new ByteArrayContent(reference), "reference", "reference.wav" },
			{ new ByteArrayContent(user), "user", "user.wav" }
		};

		var baseUrl = _settings.PitchAssessmentServiceBaseUrl.TrimEnd('/');
		using var response = await httpClient.PostAsync($"{baseUrl}/compare", form, cancellationToken);
		var raw = await response.Content.ReadAsStringAsync(cancellationToken);
		response.EnsureSuccessStatusCode();

		using var doc = JsonDocument.Parse(raw);
		var score = doc.RootElement.GetProperty("score").GetDecimal();
		return new SpeechAssessmentResult(Math.Clamp(score, 0m, 100m), raw);
	}

	// Average AccuracyScore of the best hypothesis of every recognized segment.
	private static decimal ParseAccuracy(string raw)
	{
		using var doc = JsonDocument.Parse(raw);
		var segments = doc.RootElement.ValueKind == JsonValueKind.Array
			? doc.RootElement.EnumerateArray().ToList()
			: [doc.RootElement];

		var scores = new List<decimal>();
		foreach (var segment in segments)
		{
			if (segment.TryGetProperty("RecognitionStatus", out var status) && status.GetString() != "Success")
				continue;
			if (!segment.TryGetProperty("NBest", out var nBest) || nBest.GetArrayLength() == 0)
				continue;
			if (nBest[0].TryGetProperty("PronunciationAssessment", out var pa)
				&& pa.TryGetProperty("AccuracyScore", out var accuracy))
				scores.Add(accuracy.GetDecimal());
		}

		return scores.Count == 0 ? 0m : Math.Round(scores.Average(), 2);
	}
}
