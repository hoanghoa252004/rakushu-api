using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.Speaker;
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
	public SpeakerId? SpeakerId { get; private set; }

	// Optional information identified or edited by the Curator
	public string? Name { get; private set; }
	public string? Description { get; private set; }

	// NAVIGATION PROPERTIES
	// Transcript
	public Transcript Transcript { get; private set; } = null!;

	// Speaker
	public Speaker? Speaker { get; private set; }

	// TranscriptSegments
	private readonly List<TranscriptSegment.TranscriptSegment> _transcriptSegments = new();
	public IReadOnlyCollection<TranscriptSegment.TranscriptSegment> TranscriptSegments => _transcriptSegments.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private TranscriptSpeaker() { }


	private TranscriptSpeaker(
		TranscriptSpeakerId id,
		SpeakerId? speakerId,
		TranscriptId transcriptId,
		string label)
		: base(id)
	{
		TranscriptId = transcriptId;
		SpeakerId = speakerId;
		Label = label;
	}

	public static Result<TranscriptSpeaker> Create(
		TranscriptId transcriptId,
		SpeakerId? speakerId,
		string label)
	{
		return Result.Success(new TranscriptSpeaker(
			TranscriptSpeakerId.Create(),
			speakerId,
			transcriptId,
			label));
	}
}