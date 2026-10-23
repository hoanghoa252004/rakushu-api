using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.ASR;

internal sealed class AwsAsrOutputReader : IAsrOutputReader
{
	private readonly IStorageService _storageService;

	public AwsAsrOutputReader(
		IStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task<string> ReadFullTextAsync(
		string storageKey,
		CancellationToken cancellationToken)
	{
		await using var stream = await _storageService.StreamingReadAsync(
			storageKey,
			cancellationToken);

		using var textReader = new StreamReader(stream);
		using var jsonReader = new JsonTextReader(textReader);

		while (await jsonReader.ReadAsync(cancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (jsonReader.TokenType != JsonToken.PropertyName ||
				jsonReader.Value?.ToString() != "transcripts")
			{
				continue;
			}

			if (!await jsonReader.ReadAsync(cancellationToken) ||
				jsonReader.TokenType != JsonToken.StartArray)
			{
				continue;
			}

			while (await jsonReader.ReadAsync(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (jsonReader.TokenType == JsonToken.EndArray)
					break;

				if (jsonReader.TokenType != JsonToken.StartObject)
					continue;

				var transcript = await JObject.LoadAsync(
					jsonReader,
					cancellationToken);

				var fullText = transcript["transcript"]?.Value<string>();

				if (fullText is not null)
					return fullText;
			}
		}

		throw new InvalidDataException(
			$"No transcript text found in S3 object '{storageKey}'.");
	}

	public async Task<IReadOnlyList<TranscribeSpeakerAsrReadDto>> ReadSpeakersAsync(
		string storageKey,
		CancellationToken cancellationToken)
	{
		var labels = new HashSet<string>(StringComparer.Ordinal);

		await using var stream = await _storageService.StreamingReadAsync(
			storageKey,
			cancellationToken);

		using var textReader = new StreamReader(stream);
		using var jsonReader = new JsonTextReader(textReader);

		while (await jsonReader.ReadAsync(cancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (jsonReader.TokenType != JsonToken.PropertyName ||
				jsonReader.Value?.ToString() != "speaker_labels")
			{
				continue;
			}

			if (!await jsonReader.ReadAsync(cancellationToken) ||
				jsonReader.TokenType != JsonToken.StartObject)
			{
				continue;
			}

			while (await jsonReader.ReadAsync(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (jsonReader.TokenType == JsonToken.EndObject)
					break;

				if (jsonReader.TokenType != JsonToken.PropertyName ||
					jsonReader.Value?.ToString() != "segments")
				{
					continue;
				}

				if (!await jsonReader.ReadAsync(cancellationToken) ||
					jsonReader.TokenType != JsonToken.StartArray)
				{
					continue;
				}

				while (await jsonReader.ReadAsync(cancellationToken))
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (jsonReader.TokenType == JsonToken.EndArray)
						break;

					if (jsonReader.TokenType != JsonToken.StartObject)
						continue;

					var segment = await JObject.LoadAsync(
						jsonReader,
						cancellationToken);

					var label = segment["speaker_label"]?.Value<string>();

					if (!string.IsNullOrWhiteSpace(label))
						labels.Add(label);
				}

				return labels
					.OrderBy(x => x, StringComparer.Ordinal)
					.Select(x => new TranscribeSpeakerAsrReadDto(x))
					.ToArray();
			}
		}

		return Array.Empty<TranscribeSpeakerAsrReadDto>();
	}

	public async IAsyncEnumerable<TranscribeItemAsrReadDto> ReadItemsAsync(
	string storageKey,
	[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		await using var stream = await _storageService.StreamingReadAsync(
			storageKey,
			cancellationToken);

		using var textReader = new StreamReader(stream);
		using var jsonReader = new JsonTextReader(textReader);

		var sequence = 0;
		var foundItems = false;

		while (await jsonReader.ReadAsync(cancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();

			// Chỉ tìm mảng results.items ở đúng cấp độ.
			// Bỏ qua results.speaker_labels.segments[].items.
			if (jsonReader.TokenType != JsonToken.PropertyName ||
				!string.Equals(
					jsonReader.Path,
					"results.items",
					StringComparison.Ordinal))
			{
				continue;
			}

			if (!await jsonReader.ReadAsync(cancellationToken) ||
				jsonReader.TokenType != JsonToken.StartArray)
			{
				throw new InvalidDataException(
					$"Property 'results.items' is not an array in '{storageKey}'.");
			}

			foundItems = true;

			while (await jsonReader.ReadAsync(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (jsonReader.TokenType == JsonToken.EndArray)
					break;

				if (jsonReader.TokenType != JsonToken.StartObject)
					continue;

				var item = await JObject.LoadAsync(
					jsonReader,
					cancellationToken);

				var dto = ParseItem(item, ++sequence);

				if (dto is not null)
					yield return dto;
			}

			// Đã đọc xong results.items.
			yield break;
		}

		if (!foundItems)
		{
			throw new InvalidDataException(
				$"No results.items array found in '{storageKey}'.");
		}
	}

	private static TranscribeItemAsrReadDto? ParseItem(
		JObject item,
		int sequence)
	{
		var type = item["type"]?.Value<string>();

		var alternative = item["alternatives"]?
			.FirstOrDefault();

		var text = alternative?["content"]?.Value<string>();

		if (string.IsNullOrWhiteSpace(type) || text is null)
			return null;

		var startTime = ParseTime(item["start_time"]);
		var endTime = ParseTime(item["end_time"]);

		decimal? confidence = null;

		var confidenceText = alternative?["confidence"]?.Value<string>();

		if (decimal.TryParse(
			confidenceText,
			NumberStyles.Float,
			CultureInfo.InvariantCulture,
			out var parsedConfidence))
		{
			confidence = parsedConfidence;
		}

		return new TranscribeItemAsrReadDto(
			Sequence: sequence,
			Text: text,
			Type: type,
			StartTime: startTime,
			EndTime: endTime,
			Confidence: confidence,
			SpeakerLabel: item["speaker_label"]?.Value<string>());
	}

	private static TimeSpan? ParseTime(JToken? token)
	{
		if (!double.TryParse(
			token?.Value<string>(),
			NumberStyles.Float,
			CultureInfo.InvariantCulture,
			out var seconds))
		{
			return null;
		}

		return TimeSpan.FromSeconds(seconds);
	}
}