using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Subtitle.SubtitleSegment;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

public sealed partial class TranscriptSegment : Entity<TranscriptSegmentId>
{
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public TranscriptId TranscriptId { get; private set; } = null!;
	public TranscriptSpeakerId TranscriptSpeakerId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	// Transcript
	public Transcript Transcript { get; private set; } = null!;

	// TranscriptSpeaker
	public TranscriptSpeaker.TranscriptSpeaker TranscriptSpeaker { get; private set; } = null!;

	// TranscriptItems
	private readonly List<TranscriptItem.TranscriptItem> _transcriptItems = new();
	public IReadOnlyCollection<TranscriptItem.TranscriptItem> TranscriptItems => _transcriptItems.AsReadOnly();

	// Bunsetsu
	private readonly List<Bunsetsu.Bunsetsu> _bunsetsu = new();
	public IReadOnlyCollection<Bunsetsu.Bunsetsu> Bunsetsu => _bunsetsu.AsReadOnly();

	// LearningUnits
	private readonly List<LearningUnit.LearningUnit> _learningUnits = new();
	public IReadOnlyCollection<LearningUnit.LearningUnit> LearningUnits => _learningUnits.AsReadOnly();

	// SubtitleSegment
	public SubtitleSegment SubtitleSegment { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private TranscriptSegment() { }

	private TranscriptSegment(
		TranscriptSegmentId id,
		TranscriptId transcriptId,
		TranscriptSpeakerId transcriptSpeakerId,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence)
		: base(id)
	{
		TranscriptId = transcriptId;
		TranscriptSpeakerId = transcriptSpeakerId;
		Text = text;
		StartTime = startTime;
		EndTime = endTime;
		Sequence = sequence;
	}

	public static Result<TranscriptSegment> Create(
		TranscriptId transcriptId,
		TranscriptSpeakerId transcriptSpeakerId,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence)
	{
		return Result.Success(new TranscriptSegment(
			TranscriptSegmentId.Create(),
			transcriptId,
			transcriptSpeakerId,
			text,
			startTime,
			endTime,
			sequence));
	}

	public Result Update(
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset updatedAt)
	{
		Text = text;
		StartTime = startTime;
		EndTime = endTime;
		Sequence = sequence;
		return Result.Success();
	}

	public Result<Bunsetsu.Bunsetsu> AddBunsetsu(
		string text,
		int startIndex,
		int endIndex,
		int sequence)
	{
		var result = Material.Bunsetsu.Bunsetsu.Create(
			Id,
			text,
			startIndex,
			endIndex,
			sequence);

		if (result.IsFailure)
			return result;

		_bunsetsu.Add(result.Value);
		return result;
	}

	public Result RemoveBunsetsu(BunsetsuId bunsetsuId)
	{
		var item = _bunsetsu.FirstOrDefault(b => b.Id == bunsetsuId);
		if (item is null)
			return Result.Failure(BunsetsuErrors.NotFound);

		_bunsetsu.Remove(item);
		return Result.Success();
	}

	internal void SetSequence(int newSequence)
	{
		Sequence = newSequence;
	}

	internal void IncrementSequence()
	{
		Sequence++;
	}

	internal void DecrementSequence()
	{
		Sequence--;
	}

	public Result<TranscriptItem.TranscriptItem> AddItem(
	string text,
	TranscriptItemType type,
	TimeSpan startTime,
	TimeSpan endTime,
	decimal confidence,
	int sequence)
	{
		var result = TranscriptItem.TranscriptItem.Create(
			Id,
			text,
			type,
			startTime,
			endTime,
			confidence,
			sequence);

		if (result.IsFailure)
			return result;

		_transcriptItems.Add(result.Value);

		return result;
	}


}