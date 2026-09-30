using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

public sealed partial class Bunsetsu : Entity<BunsetsuId>
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

	public static Result<Bunsetsu> Create(
		TranscriptSegmentId transcriptSegmentId,
		string text,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		return Result.Success(new Bunsetsu(
			BunsetsuId.Create(),
			transcriptSegmentId,
			text,
			startIndex,
			endIndex,
			sequence,
			createdAt,
			updatedAt));
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
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result<Token.Token> AddToken(
		string surface,
		string lemma,
		string reading,
		LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeechId japanesePartOfSpeechId,
		LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeechId universalPartOfSpeechId,
		LinguisticMetadata.DependencyRelationship.DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		var result = Token.Token.Create(
			Id,
			surface,
			lemma,
			reading,
			japanesePartOfSpeechId,
			universalPartOfSpeechId,
			dependencyRelationshipId,
			startIndex,
			endIndex,
			sequence,
			createdAt,
			updatedAt);

		if (result.IsFailure)
			return result;

		_tokens.Add(result.Value);
		UpdatedAt = updatedAt;
		return result;
	}

	public Result RemoveToken(Token.TokenId tokenId)
	{
		var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
		if (token is null)
			return Result.Failure(Token.TokenErrors.NotFound);

		_tokens.Remove(token);
		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}
}