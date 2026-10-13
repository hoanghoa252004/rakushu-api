using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.Transcript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;

public sealed class TranscriptSpeaker : Entity<TranscriptSpeakerId>
{
	public TranscriptId TranscriptId { get; private set; } = null!;

	// Speaker label returned by AWS Transcribe, e.g. speaker_0
	public string Label { get; private set; } = null!;

	// Optional information identified or edited by the Curator
	public string? Name { get; private set; }
	public string? Description { get; private set; }

	// NAVIGATION PROPERTIES
	// Transcript
	public Transcript Transcript { get; private set; } = null!;

	// TranscriptSegments
	private readonly List<TranscriptSegment.TranscriptSegment> _transcriptSegments = new();
	public IReadOnlyCollection<TranscriptSegment.TranscriptSegment> TranscriptSegments => _transcriptSegments.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private TranscriptSpeaker() { }
}