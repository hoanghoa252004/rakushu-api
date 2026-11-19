using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rakushu.Application.Abstractions.Infrastructure.JobLog;

public static class JobLogSerializer
{
	private static readonly JsonSerializerOptions Options = new()
	{
		ReferenceHandler = ReferenceHandler.IgnoreCycles,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	/// <summary>Serializes a value for a job log input/output; never throws.</summary>
	public static string? Serialize(object? value)
	{
		if (value is null)
			return null;
		if (value is string s)
			return s;

		try
		{
			return JsonSerializer.Serialize(value, value.GetType(), Options);
		}
		catch (Exception ex)
		{
			return $"<unserializable {value.GetType().Name}: {ex.Message}>";
		}
	}
}
