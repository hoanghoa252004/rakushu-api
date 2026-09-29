using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Plan.PlanEntitlement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Transcript;

public sealed class Transcript : Entity<TranscriptId>
{
	public VideoId VideoId { get; private set; } = null!;
	public string FullText { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get;private set; }


	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// TranscriptSegments
	private readonly List<TranscriptSegment.TranscriptSegment> _transcriptSegments = new();
	public IReadOnlyCollection<TranscriptSegment.TranscriptSegment> TranscriptSegments => _transcriptSegments.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Transcript()
	{
	}

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

}
