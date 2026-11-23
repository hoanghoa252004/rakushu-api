using System.Text.Json;
using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerVideoProgress;

public sealed class LearnerVideoProgress : AggregateRoot<LearnerVideoProgressId>
{
	public const double MinimumValidIntervalSeconds = 1.0;
	public const double CompletionThreshold = 0.90;
	private const double MergeToleranceSeconds = 1.0;

	public UserId UserId { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;
	public double LastWatchedSecond { get; private set; }
	public double TotalWatchedSeconds { get; private set; }
	public string WatchedIntervalsJson { get; private set; } = "[]";
	public double CompletionRate { get; private set; }
	public bool IsCompleted { get; private set; }
	public DateTimeOffset LastWatchedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public Video Video { get; private set; } = null!;

	private LearnerVideoProgress()
	{
	}

	private LearnerVideoProgress(LearnerVideoProgressId id) : base(id)
	{
	}

	public static Result<LearnerVideoProgress> Create(
		UserId userId,
		VideoId videoId,
		DateTimeOffset now)
	{
		if (userId is null || userId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerVideoProgress>(LearnerVideoProgressErrors.InvalidUserId);
		}

		if (videoId is null || videoId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerVideoProgress>(LearnerVideoProgressErrors.InvalidVideoId);
		}

		return Result.Success(new LearnerVideoProgress(LearnerVideoProgressId.Create())
		{
			UserId = userId,
			VideoId = videoId,
			LastWatchedSecond = 0,
			TotalWatchedSeconds = 0,
			WatchedIntervalsJson = "[]",
			CompletionRate = 0,
			IsCompleted = false,
			LastWatchedAt = now
		});
	}

	/// <summary>
	/// Records a playback heartbeat and merges the watched interval [rangeStartSeconds, rangeEndSeconds]
	/// using Interval Union if the duration meets the MinimumValidIntervalSeconds (>= 1.0s) dwell guard.
	/// Returns true when the range is valid for segment/vocabulary scanning, false when discarded by the dwell guard.
	/// </summary>
	public bool RecordPlayback(
		double currentPositionSeconds,
		double? rangeStartSeconds,
		double? rangeEndSeconds,
		double videoDurationSeconds,
		DateTimeOffset now)
	{
		var effectiveDuration = videoDurationSeconds > 0 ? videoDurationSeconds : 0;
		var clampedCurrentPos = Math.Max(0, currentPositionSeconds);
		if (effectiveDuration > 0 && clampedCurrentPos > effectiveDuration)
		{
			clampedCurrentPos = effectiveDuration;
		}

		LastWatchedSecond = Math.Round(clampedCurrentPos, 2);
		LastWatchedAt = now;

		if (!rangeStartSeconds.HasValue || !rangeEndSeconds.HasValue)
		{
			return false;
		}

		var start = Math.Max(0, Math.Min(rangeStartSeconds.Value, rangeEndSeconds.Value));
		var end = Math.Max(0, Math.Max(rangeStartSeconds.Value, rangeEndSeconds.Value));

		if (effectiveDuration > 0)
		{
			start = Math.Min(start, effectiveDuration);
			end = Math.Min(end, effectiveDuration);
		}

		// Validation Guard: Rapid seeking / scrubbing (< 1.0s) only updates LastWatchedSecond for Resume
		if (end - start < MinimumValidIntervalSeconds)
		{
			return false;
		}

		var intervals = ParseIntervals(WatchedIntervalsJson);
		intervals.Add([Math.Round(start, 2), Math.Round(end, 2)]);

		var merged = MergeIntervals(intervals);
		WatchedIntervalsJson = SerializeIntervals(merged);

		var uniqueSeconds = merged.Sum(iv => Math.Max(0, iv[1] - iv[0]));
		TotalWatchedSeconds = Math.Round(uniqueSeconds, 2);

		if (effectiveDuration > 0)
		{
			CompletionRate = Math.Round(Math.Min(1.0, TotalWatchedSeconds / effectiveDuration), 4);
		}

		if (CompletionRate >= CompletionThreshold)
		{
			IsCompleted = true;
		}

		return true;
	}

	public IReadOnlyList<double[]> GetWatchedIntervals() => ParseIntervals(WatchedIntervalsJson);

	public static List<double[]> ParseIntervals(string? json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return [];
		}

		try
		{
			var raw = JsonSerializer.Deserialize<List<double[]>>(json);
			if (raw is null)
			{
				return [];
			}

			return raw
				.Where(iv => iv is not null && iv.Length >= 2 && iv[1] > iv[0])
				.Select(iv => new[] { Math.Round(Math.Max(0, iv[0]), 2), Math.Round(Math.Max(0, iv[1]), 2) })
				.ToList();
		}
		catch
		{
			return [];
		}
	}

	public static List<double[]> MergeIntervals(IEnumerable<double[]> rawIntervals)
	{
		var sorted = rawIntervals
			.Where(iv => iv is not null && iv.Length >= 2)
			.Select(iv =>
			{
				var s = Math.Max(0, Math.Min(iv[0], iv[1]));
				var e = Math.Max(0, Math.Max(iv[0], iv[1]));
				return new[] { Math.Round(s, 2), Math.Round(e, 2) };
			})
			.Where(iv => iv[1] > iv[0])
			.OrderBy(iv => iv[0])
			.ThenBy(iv => iv[1])
			.ToList();

		if (sorted.Count == 0)
		{
			return [];
		}

		var merged = new List<double[]> { sorted[0] };
		for (int i = 1; i < sorted.Count; i++)
		{
			var current = sorted[i];
			var prev = merged[^1];

			if (current[0] <= prev[1] + MergeToleranceSeconds)
			{
				prev[1] = Math.Round(Math.Max(prev[1], current[1]), 2);
			}
			else
			{
				merged.Add([current[0], current[1]]);
			}
		}

		return merged;
	}

	private static string SerializeIntervals(List<double[]> intervals) =>
		JsonSerializer.Serialize(intervals);
}
