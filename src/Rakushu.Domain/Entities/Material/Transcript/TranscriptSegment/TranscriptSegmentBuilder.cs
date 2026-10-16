using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

public sealed class TranscriptSegmentBuilder
{
	private static readonly TimeSpan MaxPause =
		TimeSpan.FromMilliseconds(700);

	private static readonly TimeSpan TargetMinDuration =
		TimeSpan.FromSeconds(2);

	private static readonly TimeSpan TargetMaxDuration =
		TimeSpan.FromSeconds(8);

	private static readonly TimeSpan PreferredMaxDuration =
		TimeSpan.FromSeconds(12);

	private static readonly HashSet<string> SentenceTerminators =
		new(StringComparer.Ordinal)
		{
			"。",
			"！",
			"？"
		};

	public IReadOnlyList<TranscriptSegmentDraft> Build(
		IEnumerable<TranscribeItemBuildDto> sourceItems)
	{
		ArgumentNullException.ThrowIfNull(sourceItems);

		var items = sourceItems
			.OrderBy(x => x.Sequence)
			.ToList();

		if (items.Count == 0)
			return [];

		var segments = new List<TranscriptSegmentDraft>();
		var currentItems = new List<TranscribeItemBuildDto>();

		foreach (var item in items)
		{
			// Punctuation has no timestamp in AWS Transcribe.
			// Attach it to the preceding item instead of creating a segment.
			if (IsPunctuation(item))
			{
				if (currentItems.Count > 0)
				{
					currentItems.Add(item);

					if (IsSentenceTerminator(item.Text))
					{
						FlushSegment(currentItems, segments);
					}
				}

				continue;
			}

			if (string.IsNullOrWhiteSpace(item.Text))
				continue;

			if (currentItems.Count == 0)
			{
				currentItems.Add(item);
				continue;
			}

			var previousTimedItem = FindLastTimedItem(currentItems);

			// Rule 1: split when the speaker changes.
			if (HasSpeakerChanged(previousTimedItem, item))
			{
				FlushSegment(currentItems, segments);
				currentItems.Add(item);
				continue;
			}

			// Rule 2: split when the pause is greater than 700 ms.
			if (HasLongPause(previousTimedItem, item))
			{
				FlushSegment(currentItems, segments);
				currentItems.Add(item);
				continue;
			}

			var currentDuration = GetDuration(currentItems, item);

			// Rule 3: aim for 2-8 seconds, but don't cut a sentence
			// just because it exceeds 8 seconds.
			//
			// We only use the duration as a signal here. The actual
			// split should occur at a linguistic boundary, such as
			// punctuation or a pause, rather than arbitrarily mid-sentence.
			if (ShouldPreferBoundary(currentItems, item, currentDuration))
			{
				// There is no reliable linguistic-boundary signal on
				// an individual ASR item yet. Keep the current sentence
				// intact and wait for punctuation or a pause.
			}

			currentItems.Add(item);

			// Rule 4: sentence-ending punctuation closes the segment.
			// Punctuation may be part of the current item text.
			if (ContainsSentenceTerminator(item.Text))
			{
				FlushSegment(currentItems, segments);
			}
		}

		FlushSegment(currentItems, segments);

		return segments
			.OrderBy(x => x.StartTime)
			.Select((segment, index) => segment with
			{
				Sequence = index + 1
			})
			.ToList();
	}

	private static bool IsPunctuation(
		TranscribeItemBuildDto item)
	{
		return string.Equals(
			item.Type,
			TranscriptItemType.Punctuation.ToString(),
			StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsSentenceTerminator(string text)
	{
		return SentenceTerminators.Contains(text.Trim());
	}

	private static bool ContainsSentenceTerminator(string text)
	{
		return text.Any(c =>
			c is '。' or '！' or '？');
	}

	private static bool HasSpeakerChanged(
		TranscribeItemBuildDto? previous,
		TranscribeItemBuildDto current)
	{
		if (previous is null)
			return false;

		// Missing speaker labels should not be interpreted as a
		// speaker change.
		if (string.IsNullOrWhiteSpace(previous.SpeakerLabel) ||
			string.IsNullOrWhiteSpace(current.SpeakerLabel))
		{
			return false;
		}

		return !string.Equals(
			previous.SpeakerLabel,
			current.SpeakerLabel,
			StringComparison.Ordinal);
	}

	private static bool HasLongPause(
		TranscribeItemBuildDto? previous,
		TranscribeItemBuildDto current)
	{
		if (previous?.EndTime is not TimeSpan previousEnd ||
			current.StartTime is not TimeSpan currentStart)
		{
			return false;
		}

		return currentStart - previousEnd > MaxPause;
	}

	private static TimeSpan GetDuration(
		IReadOnlyCollection<TranscribeItemBuildDto> currentItems,
		TranscribeItemBuildDto nextItem)
	{
		var first = currentItems.FirstOrDefault(x => x.StartTime.HasValue);
		var last = nextItem.EndTime.HasValue
			? nextItem
			: currentItems.LastOrDefault(x => x.EndTime.HasValue);

		if (first?.StartTime is not TimeSpan start ||
			last?.EndTime is not TimeSpan end)
		{
			return TimeSpan.Zero;
		}

		return end - start;
	}

	private static bool ShouldPreferBoundary(
		IReadOnlyCollection<TranscribeItemBuildDto> currentItems,
		TranscribeItemBuildDto nextItem,
		TimeSpan duration)
	{
		// Duration is only a heuristic. Do not split at arbitrary
		// word boundaries just to satisfy a duration target.
		return duration >= TargetMinDuration &&
			   duration >= TargetMaxDuration &&
			   currentItems.Count > 0;
	}

	private static TranscribeItemBuildDto? FindLastTimedItem(
		IReadOnlyList<TranscribeItemBuildDto> items)
	{
		for (var i = items.Count - 1; i >= 0; i--)
		{
			if (items[i].StartTime.HasValue ||
				items[i].EndTime.HasValue)
			{
				return items[i];
			}
		}

		return null;
	}

	private static void FlushSegment(
		List<TranscribeItemBuildDto> currentItems,
		List<TranscriptSegmentDraft> segments)
	{
		if (currentItems.Count == 0)
			return;

		var timedItems = currentItems
			.Where(x => x.StartTime.HasValue && x.EndTime.HasValue)
			.ToList();

		if (timedItems.Count == 0)
		{
			currentItems.Clear();
			return;
		}

		var text = BuildText(currentItems);

		if (!string.IsNullOrWhiteSpace(text))
		{
			var speakerLabel = timedItems
				.Select(x => x.SpeakerLabel)
				.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

			segments.Add(new TranscriptSegmentDraft(
				Text: text,
				StartTime: timedItems.Min(x => x.StartTime!.Value),
				EndTime: timedItems.Max(x => x.EndTime!.Value),
				Sequence: segments.Count + 1,
				SpeakerLabel: speakerLabel,
				Items: currentItems.ToList()));
		}

		currentItems.Clear();
	}

	private static string BuildText(
		IEnumerable<TranscribeItemBuildDto> items)
	{
		var builder = new StringBuilder();

		foreach (var item in items)
		{
			var text = item.Text;

			if (string.IsNullOrEmpty(text))
				continue;

			if (IsPunctuation(item))
			{
				// Japanese punctuation attaches directly to the
				// preceding text: "こんにちは。"
				builder.Append(text);
				continue;
			}

			// AWS Transcribe normally emits Japanese words/tokens.
			// Avoid inserting spaces between Japanese tokens.
			builder.Append(text);
		}

		return builder.ToString().Trim();
	}
}

public sealed record TranscribeItemBuildDto(
	int Sequence,
	string Text,
	string Type,
	TimeSpan? StartTime,
	TimeSpan? EndTime,
	decimal? Confidence,
	string? SpeakerLabel);

public sealed record TranscriptSegmentDraft(
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int Sequence,
	string? SpeakerLabel,
	IReadOnlyList<TranscribeItemBuildDto> Items);