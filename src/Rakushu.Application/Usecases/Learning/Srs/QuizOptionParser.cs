using System.Text.Json;

namespace Rakushu.Application.Usecases.Learning.Srs;

public static class QuizOptionParser
{
	public static List<QuizOptionDto> Parse(string? choicesJson)
	{
		if (string.IsNullOrWhiteSpace(choicesJson))
			return [];

		try
		{
			using var doc = JsonDocument.Parse(choicesJson);
			if (doc.RootElement.ValueKind != JsonValueKind.Array)
				return [];

			var list = new List<QuizOptionDto>();
			foreach (var element in doc.RootElement.EnumerateArray())
			{
				if (element.ValueKind == JsonValueKind.Object)
				{
					string id = element.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
					string text = element.TryGetProperty("text", out var textProp) ? textProp.GetString() ?? "" : "";
					list.Add(new QuizOptionDto(id, text));
				}
				else if (element.ValueKind == JsonValueKind.String)
				{
					string val = element.GetString() ?? "";
					list.Add(new QuizOptionDto(val, val));
				}
			}

			return list;
		}
		catch
		{
			return [];
		}
	}
}
