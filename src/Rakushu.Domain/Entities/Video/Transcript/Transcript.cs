using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Video.Transcript;

public sealed partial class Transcript : Entity<TranscriptId>
{
	public VideoId VideoId { get; private set; } = null!;
	public string FullText { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// TranscriptSegments
	private readonly List<TranscriptSegment.TranscriptSegment> _transcriptSegments = new();
	public IReadOnlyCollection<TranscriptSegment.TranscriptSegment> TranscriptSegments => _transcriptSegments.AsReadOnly();

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

	public Result<TranscriptSegment.TranscriptSegment> AddSegment(
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		var segmentResult = TranscriptSegment.TranscriptSegment.Create(
			Id,
			text,
			startTime,
			endTime,
			sequence,
			createdAt,
			updatedAt);

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

		_transcriptSegments.Remove(segment);
		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}
}