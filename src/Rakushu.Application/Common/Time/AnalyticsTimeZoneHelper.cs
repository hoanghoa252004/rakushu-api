namespace Rakushu.Application.Common.Time;

public static class AnalyticsTimeZoneHelper
{
	public const string DefaultTimeZoneId = "Asia/Ho_Chi_Minh";

	public static TimeZoneInfo ResolveTimeZone(string? timeZoneId)
	{
		var candidate = string.IsNullOrWhiteSpace(timeZoneId)
			? DefaultTimeZoneId
			: timeZoneId.Trim();

		if (TryFindTimeZone(candidate, out var resolved))
			return resolved;

		if (TimeZoneInfo.TryConvertIanaIdToWindowsId(candidate, out var windowsId)
			&& TryFindTimeZone(windowsId, out resolved))
		{
			return resolved;
		}

		if (TimeZoneInfo.TryConvertWindowsIdToIanaId(candidate, out var ianaId)
			&& TryFindTimeZone(ianaId, out resolved))
		{
			return resolved;
		}

		// Final fallback to Asia/Ho_Chi_Minh if an invalid timezone was passed before falling back to UTC.
		if (!string.Equals(candidate, DefaultTimeZoneId, StringComparison.OrdinalIgnoreCase))
		{
			return ResolveTimeZone(DefaultTimeZoneId);
		}

		return TimeZoneInfo.Utc;
	}

	public static DateOnly ToLocalDate(DateTimeOffset utcTimestamp, TimeZoneInfo timeZone)
	{
		var local = TimeZoneInfo.ConvertTime(utcTimestamp, timeZone);
		return DateOnly.FromDateTime(local.DateTime);
	}

	public static DateOnly GetLocalToday(DateTimeOffset utcNow, TimeZoneInfo timeZone)
		=> ToLocalDate(utcNow, timeZone);

	public static (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetUtcBoundsForLocalDate(
		DateOnly localDate,
		TimeZoneInfo timeZone)
	{
		var startUtc = ConvertLocalMidnightToUtc(localDate, timeZone);
		var endUtc = ConvertLocalMidnightToUtc(localDate.AddDays(1), timeZone);
		return (startUtc, endUtc);
	}

	public static (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetUtcBoundsForYear(
		int year,
		TimeZoneInfo timeZone)
	{
		var startUtc = ConvertLocalMidnightToUtc(new DateOnly(year, 1, 1), timeZone);
		var endUtc = ConvertLocalMidnightToUtc(new DateOnly(year + 1, 1, 1), timeZone);
		return (startUtc, endUtc);
	}

	private static DateTimeOffset ConvertLocalMidnightToUtc(DateOnly localDate, TimeZoneInfo timeZone)
	{
		var unspecified = DateTime.SpecifyKind(localDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
		var offset = timeZone.GetUtcOffset(unspecified);
		return new DateTimeOffset(unspecified, offset).ToUniversalTime();
	}

	private static bool TryFindTimeZone(string id, out TimeZoneInfo timeZone)
	{
		try
		{
			timeZone = TimeZoneInfo.FindSystemTimeZoneById(id);
			return true;
		}
		catch (TimeZoneNotFoundException)
		{
			timeZone = TimeZoneInfo.Utc;
			return false;
		}
		catch (InvalidTimeZoneException)
		{
			timeZone = TimeZoneInfo.Utc;
			return false;
		}
	}
}
