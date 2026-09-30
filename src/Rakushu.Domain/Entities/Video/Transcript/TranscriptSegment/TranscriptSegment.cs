using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;

namespace Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

public sealed partial class TranscriptSegment : Entity<TranscriptSegmentId>
{
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public TranscriptId TranscriptId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Transcript
	public Transcript Transcript { get; private set; } = null!;

	// Bunsetsu
	private readonly List<Bunsetsu> _bunsetsu = new();
	public IReadOnlyCollection<Bunsetsu> Bunsetsu => _bunsetsu.AsReadOnly();

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
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		TranscriptId = transcriptId;
		Text = text;
		StartTime = startTime;
		EndTime = endTime;
		Sequence = sequence;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<TranscriptSegment> Create(
		TranscriptId transcriptId,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		return Result.Success(new TranscriptSegment(
			TranscriptSegmentId.Create(),
			transcriptId,
			text,
			startTime,
			endTime,
			sequence,
			createdAt,
			updatedAt));
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
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result<Bunsetsu> AddBunsetsu(
		string text,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		var result = LearningUnit.Bunsetsu.Bunsetsu.Create(
			Id,
			text,
			startIndex,
			endIndex,
			sequence,
			createdAt,
			updatedAt);

		if (result.IsFailure)
			return result;

		_bunsetsu.Add(result.Value);
		UpdatedAt = updatedAt;
		return result;
	}

	public Result RemoveBunsetsu(LearningUnit.Bunsetsu.BunsetsuId bunsetsuId)
	{
		var item = _bunsetsu.FirstOrDefault(b => b.Id == bunsetsuId);
		if (item is null)
			return Result.Failure(LearningUnit.Bunsetsu.BunsetsuErrors.NotFound);

		_bunsetsu.Remove(item);
		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}
}