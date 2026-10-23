using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Material.Transcript;

public sealed partial class Transcript : Entity<TranscriptId>
{
	public VideoId VideoId { get; private set; } = null!;
	public string FullText { get; private set; } = null!;
	public TranscriptSource SourceType { get; private set; }
	public string? StorageKey { get; private set; }
	public bool IsReviewed { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Video
	public Video.Video Video { get; private set; } = null!;

	// TranscriptSegments
	private readonly List<TranscriptSegment.TranscriptSegment> _transcriptSegments = new();
	public IReadOnlyCollection<TranscriptSegment.TranscriptSegment> TranscriptSegments => _transcriptSegments.AsReadOnly();

	// TranscriptSpeakers
	private readonly List<TranscriptSpeaker.TranscriptSpeaker> _transcriptSpeakers = new();
	public IReadOnlyCollection<TranscriptSpeaker.TranscriptSpeaker> TranscriptSpeakers => _transcriptSpeakers.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Transcript() { }

	private Transcript(
		TranscriptId id,
		VideoId videoId,
		string fullText,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		VideoId = videoId;
		FullText = fullText;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<Transcript> Create(
		VideoId videoId,
		string fullText,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		return Result.Success(new Transcript(
			TranscriptId.Create(),
			videoId,
			fullText,
			createdAt,
			updatedAt));
	}

	public Result Update(string fullText, DateTimeOffset updatedAt)
	{
		FullText = fullText;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public void SetStorageKey(string storageKey)
	{
		StorageKey = storageKey;
		SourceType = TranscriptSource.Official;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public Result UpdateTranscript(string fullText, string storageKey, DateTimeOffset updatedAt)
	{
		FullText = fullText;
		StorageKey = storageKey;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result<TranscriptSegment.TranscriptSegment> AddSegment(
	TranscriptSpeakerId transcriptSpeakerId,
	string text,
	TimeSpan startTime,
	TimeSpan endTime,
	int sequence,
	DateTimeOffset updatedAt)
	{
		var segmentResult = TranscriptSegment.TranscriptSegment.Create(
			Id,
			transcriptSpeakerId,
			text,
			startTime,
			endTime,
			sequence);

		if (segmentResult.IsFailure)
			return segmentResult;

		_transcriptSegments.Add(segmentResult.Value);

		UpdatedAt = updatedAt;

		return segmentResult;
	}

	public Result RemoveSegment(TranscriptSegment.TranscriptSegmentId segmentId)
	{
		var segment = _transcriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure(TranscriptSegment.TranscriptSegmentErrors.NotFound);

		var removedSequence = segment.Sequence;
		_transcriptSegments.Remove(segment);

		// Reorder remaining segments after the removed one
		var segmentsToReorder = _transcriptSegments
			.Where(s => s.Sequence > removedSequence)
			.OrderBy(s => s.Sequence)
			.ToList();

		foreach (var seg in segmentsToReorder)
		{
			seg.DecrementSequence();
		}

		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}

	public Result UpdateSegmentSequence(TranscriptSegment.TranscriptSegmentId segmentId, int newSequence)
	{
		var segment = _transcriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure(TranscriptSegment.TranscriptSegmentErrors.NotFound);

		var oldSequence = segment.Sequence;
		var maxSequence = _transcriptSegments.Count;

		if (newSequence < 1 || newSequence > maxSequence)
			return Result.Failure(TranscriptSegment.TranscriptSegmentErrors.InvalidSequence);

		if (oldSequence == newSequence)
			return Result.Success();

		// If moving forward (higher sequence)
		if (newSequence > oldSequence)
		{
			var segmentsToShift = _transcriptSegments
				.Where(s => s.Sequence > oldSequence && s.Sequence <= newSequence)
				.OrderBy(s => s.Sequence)
				.ToList();

			foreach (var seg in segmentsToShift)
			{
				seg.DecrementSequence();
			}
		}
		// If moving backward (lower sequence)
		else
		{
			var segmentsToShift = _transcriptSegments
				.Where(s => s.Sequence >= newSequence && s.Sequence < oldSequence)
				.OrderByDescending(s => s.Sequence)
				.ToList();

			foreach (var seg in segmentsToShift)
			{
				seg.IncrementSequence();
			}
		}

		segment.SetSequence(newSequence);
		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}

	public Result<TranscriptSpeaker.TranscriptSpeaker> AddSpeaker(
	string label)
	{
		var existingSpeaker = _transcriptSpeakers
			.FirstOrDefault(x => x.Label == label);

		if (existingSpeaker is not null)
			return Result.Success(existingSpeaker);

		var result = TranscriptSpeaker.TranscriptSpeaker.Create(
			Id,
			speakerId: null,
			label);

		if (result.IsFailure)
			return result;

		_transcriptSpeakers.Add(result.Value);

		return result;
	}
}