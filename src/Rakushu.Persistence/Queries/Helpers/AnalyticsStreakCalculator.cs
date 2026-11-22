namespace Rakushu.Persistence.Queries.Helpers;

public static class AnalyticsStreakCalculator
{
	public static (int CurrentStreakDays, int LongestStreakDays) ComputeStreaks(
		IReadOnlyCollection<DateOnly> distinctActiveDates,
		DateOnly localToday)
	{
		if (distinctActiveDates.Count == 0)
			return (0, 0);

		var sorted = distinctActiveDates
			.Distinct()
			.OrderBy(d => d)
			.ToList();

		int longest = 1;
		int run = 1;

		for (int i = 1; i < sorted.Count; i++)
		{
			if (sorted[i].DayNumber - sorted[i - 1].DayNumber == 1)
			{
				run++;
				if (run > longest)
					longest = run;
			}
			else
			{
				run = 1;
			}
		}

		// Current streak counts consecutive days ending at localToday or yesterday.
		var dateSet = sorted.ToHashSet();
		var anchor = dateSet.Contains(localToday)
			? localToday
			: dateSet.Contains(localToday.AddDays(-1))
				? localToday.AddDays(-1)
				: (DateOnly?)null;

		if (!anchor.HasValue)
			return (0, longest);

		int current = 0;
		var cursor = anchor.Value;
		while (dateSet.Contains(cursor))
		{
			current++;
			cursor = cursor.AddDays(-1);
		}

		return (current, longest);
	}
}
