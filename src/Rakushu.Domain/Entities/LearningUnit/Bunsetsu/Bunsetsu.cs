using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

public sealed partial class Bunsetsu : Entity<BunsetsuId>
{
	public int Sequence { get; private set; }
	public string Text { get; private set; } = null!;
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;

	// Tokens
	private readonly List<Token.Token> _tokens = new();
	public IReadOnlyCollection<Token.Token> Tokens => _tokens.AsReadOnly();

	// Outgoing dependency relations
	private readonly List<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> _outgoingRelations = new();
	public IReadOnlyCollection<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> OutgoingRelations => _outgoingRelations.AsReadOnly();

	// Incoming dependency relations
	private readonly List<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> _incomingRelations = new();
	public IReadOnlyCollection<BunsetsuDependencyRelationship.BunsetsuDependencyRelationship> IncomingRelations => _incomingRelations.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Bunsetsu() { }

	private Bunsetsu(
		BunsetsuId id,
		TranscriptSegmentId transcriptSegmentId,
		string text,
		int startIndex,
		int endIndex,
		int sequence)
		: base(id)
	{
		TranscriptSegmentId = transcriptSegmentId;
		Text = text;
		StartIndex = startIndex;
		EndIndex = endIndex;
		Sequence = sequence;
	}

	public static Result<Bunsetsu> Create(
		TranscriptSegmentId transcriptSegmentId,
		string text,
		int startIndex,
		int endIndex,
		int sequence)
	{
		return Result.Success(new Bunsetsu(
			BunsetsuId.Create(),
			transcriptSegmentId,
			text,
			startIndex,
			endIndex,
			sequence));
	}

	public Result Update(
		string text,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset updatedAt)
	{
		Text = text;
		StartIndex = startIndex;
		EndIndex = endIndex;
		Sequence = sequence;
		return Result.Success();
	}

	public Result<Token.Token> AddToken(
		string surface,
		string lemma,
		string reading,
		JapanesePartOfSpeechId japanesePartOfSpeechId,
		DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence)
	{
		var result = Token.Token.Create(
			Id,
			surface,
			lemma,
			reading,
			japanesePartOfSpeechId,
			dependencyRelationshipId,
			startIndex,
			endIndex,
			sequence);

		if (result.IsFailure)
			return result;

		_tokens.Add(result.Value);
		return result;
	}
}