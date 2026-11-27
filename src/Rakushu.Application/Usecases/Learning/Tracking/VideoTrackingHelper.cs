using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rakushu.Application.Usecases.Learning.Tracking;

public static class VideoTrackingHelper
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public static List<EncounterSourceDto> ParseEncounterSources(string? json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return [];
		}

		try
		{
			return JsonSerializer.Deserialize<List<EncounterSourceDto>>(json, JsonOptions) ?? [];
		}
		catch
		{
			return [];
		}
	}

	public static string UpsertEncounterSource(
		string? existingJson,
		EncounterSourceDto newSource,
		int maxSources = 5)
	{
		var list = ParseEncounterSources(existingJson);

		list.RemoveAll(s =>
			s.VideoId == newSource.VideoId &&
			Math.Abs(s.TimestampSeconds - newSource.TimestampSeconds) < 0.5);

		list.Insert(0, newSource);

		if (list.Count > maxSources)
		{
			list = list.Take(maxSources).ToList();
		}

		return JsonSerializer.Serialize(list, JsonOptions);
	}

	public static string FormatDuration(double totalSeconds)
	{
		if (totalSeconds <= 0 || double.IsNaN(totalSeconds))
		{
			return "00:00";
		}

		int secsTotal = (int)Math.Round(totalSeconds);
		int hrs = secsTotal / 3600;
		int mins = (secsTotal % 3600) / 60;
		int secs = secsTotal % 60;

		if (hrs > 0)
		{
			return $"{hrs:D2}:{mins:D2}:{secs:D2}";
		}

		return $"{mins:D2}:{secs:D2}";
	}
}
