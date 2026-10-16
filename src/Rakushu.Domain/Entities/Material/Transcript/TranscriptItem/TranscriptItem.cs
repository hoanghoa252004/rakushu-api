using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;

public sealed class TranscriptItem : Entity<TranscriptItemId>
{
	public int Sequence { get; private set; }
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public string Text { get; private set; } = null!;
	public TranscriptItemType Type { get; private set; }
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public decimal Confidence { get; private set; }

	// NAVIGATION PROPERTIES
	// TranscriptSegment
	public TranscriptSegment.TranscriptSegment TranscriptSegment { get; private set; } = null!;

	private TranscriptItem() { }

	private TranscriptItem(
		TranscriptItemId id,
		TranscriptSegmentId transcriptSegmentId,
		string text,
		TranscriptItemType type,
		TimeSpan startTime,
		TimeSpan endTime,
		decimal confidence,
		int sequence)
		: base(id)
	{
		TranscriptSegmentId = transcriptSegmentId;
		Text = text;
		Type = type;
		StartTime = startTime;
		EndTime = endTime;
		Confidence = confidence;
		Sequence = sequence;
	}

	public static Result<TranscriptItem> Create(
		TranscriptSegmentId transcriptSegmentId,
		string text,
		TranscriptItemType type,
		TimeSpan startTime,
		TimeSpan endTime,
		decimal confidence,
		int sequence)
	{
		if (string.IsNullOrWhiteSpace(text))
			return Result.Failure<TranscriptItem>(TranscriptItemErrors.TextNotBeEmpty);

		if (startTime < TimeSpan.Zero || endTime < startTime)
			return Result.Failure<TranscriptItem>(TranscriptItemErrors.InvalidTimeRange);

		if (sequence < 1)
			return Result.Failure<TranscriptItem>(TranscriptItemErrors.InvalidSequence);

		return Result.Success(new TranscriptItem(
			TranscriptItemId.Create(),
			transcriptSegmentId,
			text,
			type,
			startTime,
			endTime,
			confidence,
			sequence));
	}
}