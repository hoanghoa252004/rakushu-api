using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

public sealed class Bunsetsu : Entity<BunsetsuId>
{
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;

	// Tokens
	private readonly List<Token.Token> _tokens = new();
	public IReadOnlyCollection<Token.Token> Tokens => _tokens.AsReadOnly();

	// Outgoing dependency relations
	private readonly List<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> _outgoingRelations = new();
	public IReadOnlyCollection<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> OutgoingRelations
		=> _outgoingRelations.AsReadOnly();

	// Incoming dependency relations
	private readonly List<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> _incomingRelations = new();
	public IReadOnlyCollection<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> IncomingRelations
		=> _incomingRelations.AsReadOnly();


	// CONSTRUCTORS & FACTORY METHODS

	private Bunsetsu()
	{
	}

	private Bunsetsu(
		BunsetsuId id,
		TranscriptSegmentId transcriptSegmentId,
		string text,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		TranscriptSegmentId = transcriptSegmentId;
		Text = text;
		StartIndex = startIndex;
		EndIndex = endIndex;
		Sequence = sequence;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}