using System.Text.Json;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Shadowing;

namespace Rakushu.Application.Usecases.Analytics;

public static class AzureSpeechWeakWordParser
{
	private const decimal GoodScoreThreshold = 85m;
	private const decimal WarningScoreThreshold = 70m;

	public static IReadOnlyList<ShadowingWordBreakdownDto> ParseWordBreakdown(string? accuracyResponseJson)
	{
		if (string.IsNullOrWhiteSpace(accuracyResponseJson))
			return [];

		try
		{
			using var doc = JsonDocument.Parse(accuracyResponseJson);
			var words = ExtractWordNodes(doc.RootElement);
			return words.Select(w =>
			{
				var status = w.Score >= GoodScoreThreshold
					? "good"
					: w.Score >= WarningScoreThreshold
						? "warning"
						: "error";

				var tag = BuildWordTag(w.ErrorType, w.Score, status);
				return new ShadowingWordBreakdownDto(w.Word, w.Score, status, w.ErrorType, tag);
			}).ToList();
		}
		catch (JsonException)
		{
			return [];
		}
	}

	public static IReadOnlyList<ShadowingWeakWordDto> ExtractWeakWords(
		IEnumerable<ShadowingAssessmentLineInput> lines,
		int maxCount = 4)
	{
		var wordCandidates = new Dictionary<string, (decimal MinScore, string ErrorType, string? LineText, string? Meaning)>(StringComparer.Ordinal);
		var lineList = lines.ToList();

		foreach (var line in lineList)
		{
			if (string.IsNullOrWhiteSpace(line.AccuracyAssessmentResponse))
				continue;

			try
			{
				using var doc = JsonDocument.Parse(line.AccuracyAssessmentResponse);
				foreach (var node in ExtractWordNodes(doc.RootElement))
				{
					var hasExplicitError = !string.IsNullOrWhiteSpace(node.ErrorType)
						&& !string.Equals(node.ErrorType, "None", StringComparison.OrdinalIgnoreCase);

					if (!hasExplicitError && node.Score >= GoodScoreThreshold)
						continue;

					if (wordCandidates.TryGetValue(node.Word, out var existing) && existing.MinScore <= node.Score)
						continue;

					wordCandidates[node.Word] = (node.Score, node.ErrorType ?? "Mispronunciation", line.LineText, line.VietnameseMeaning);
				}
			}
			catch (JsonException)
			{
				// Ignore malformed JSON from legacy/synthetic records and rely on line-level fallback.
			}
		}

		if (wordCandidates.Count > 0)
		{
			return wordCandidates
				.OrderBy(kv => kv.Value.MinScore)
				.Take(maxCount)
				.Select(kv =>
				{
					var rounded = Math.Round(kv.Value.MinScore, 0);
					var issueLabel = FormatIssueLabel(kv.Value.ErrorType);
					return new ShadowingWeakWordDto(
						kv.Key,
						null,
						kv.Value.Meaning ?? kv.Value.LineText,
						rounded,
						kv.Value.ErrorType,
						$"{issueLabel} {rounded:0}đ");
				})
				.ToList();
		}

		// Fallback to lowest-scoring lines when no word-level Azure payload is stored.
		return lineList
			.Select(l =>
			{
				var acc = l.AccuracyScore ?? 100m;
				var pitch = l.PitchSimilarityScore ?? acc;
				var minMetric = Math.Min(acc, pitch);
				var isPitchLower = pitch < acc;
				return new { Line = l, MinMetric = minMetric, IsPitchLower = isPitchLower };
			})
			.Where(x => x.MinMetric < GoodScoreThreshold)
			.OrderBy(x => x.MinMetric)
			.Take(maxCount)
			.Select(x =>
			{
				var rounded = Math.Round(x.MinMetric, 0);
				var issueType = x.IsPitchLower ? "PitchMismatch" : "Mispronunciation";
				var label = x.IsPitchLower ? "Lệch Pitch" : "Phát âm";
				return new ShadowingWeakWordDto(
					x.Line.LineText,
					null,
					x.Line.VietnameseMeaning ?? x.Line.Feedback,
					rounded,
					issueType,
					$"{label} {rounded:0}đ");
			})
			.ToList();
	}

	private static List<(string Word, decimal Score, string? ErrorType)> ExtractWordNodes(JsonElement root)
	{
		var results = new List<(string, decimal, string?)>();
		IEnumerable<JsonElement> segments = root.ValueKind == JsonValueKind.Array
			? root.EnumerateArray().ToList()
			: [root];

		foreach (var seg in segments)
		{
			if (!TryGetPropertyInsensitive(seg, "NBest", out var nBest)
				|| nBest.ValueKind != JsonValueKind.Array
				|| nBest.GetArrayLength() == 0)
			{
				continue;
			}

			var best = nBest[0];
			if (!TryGetPropertyInsensitive(best, "Words", out var wordsEl)
				|| wordsEl.ValueKind != JsonValueKind.Array)
			{
				continue;
			}

			foreach (var wordEl in wordsEl.EnumerateArray())
			{
				if (!TryGetPropertyInsensitive(wordEl, "Word", out var textEl)
					|| textEl.ValueKind != JsonValueKind.String)
				{
					continue;
				}

				var wordText = textEl.GetString()?.Trim();
				if (string.IsNullOrWhiteSpace(wordText))
					continue;

				decimal score = 0m;
				string? errorType = null;

				if (TryGetPropertyInsensitive(wordEl, "PronunciationAssessment", out var paEl)
					&& paEl.ValueKind == JsonValueKind.Object)
				{
					score = ReadDecimal(paEl, "AccuracyScore") ?? ReadDecimal(wordEl, "AccuracyScore") ?? 0m;
					errorType = ReadString(paEl, "ErrorType") ?? ReadString(wordEl, "ErrorType");
				}
				else
				{
					score = ReadDecimal(wordEl, "AccuracyScore") ?? 0m;
					errorType = ReadString(wordEl, "ErrorType");
				}

				results.Add((wordText, Math.Round(Math.Clamp(score, 0m, 100m), 1), errorType));
			}
		}

		return results;
	}

	private static string? BuildWordTag(string? errorType, decimal score, string status)
	{
		if (status == "good")
			return null;

		var label = FormatIssueLabel(errorType);
		return $"{label} {Math.Round(score, 0):0}đ";
	}

	private static string FormatIssueLabel(string? errorType) => errorType?.Trim() switch
	{
		"Omission" => "Nuốt âm",
		"Insertion" => "Thừa âm",
		"PitchMismatch" => "Lệch Pitch",
		_ => "Lệch âm",
	};

	private static bool TryGetPropertyInsensitive(JsonElement element, string name, out JsonElement value)
	{
		if (element.ValueKind == JsonValueKind.Object)
		{
			if (element.TryGetProperty(name, out value))
				return true;

			foreach (var prop in element.EnumerateObject())
			{
				if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					value = prop.Value;
					return true;
				}
			}
		}

		value = default;
		return false;
	}

	private static decimal? ReadDecimal(JsonElement element, string name)
	{
		if (TryGetPropertyInsensitive(element, name, out var prop) && prop.ValueKind == JsonValueKind.Number)
		{
			return prop.TryGetDecimal(out var dec) ? dec : (decimal)prop.GetDouble();
		}

		return null;
	}

	private static string? ReadString(JsonElement element, string name)
	{
		return TryGetPropertyInsensitive(element, name, out var prop) && prop.ValueKind == JsonValueKind.String
			? prop.GetString()
			: null;
	}
}
