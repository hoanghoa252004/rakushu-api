using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Material.Bunsetsu;

namespace Rakushu.Domain.Entities.Material.Token;

public sealed partial class Token : Entity<TokenId>
{
	public int Sequence { get; private set; }
	public BunsetsuId BunsetsuId { get; private set; } = null!;
	public string Surface { get; private set; } = null!;
	public string Lemma { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public int HeadSequence { get; private set; }
	public JapanesePartOfSpeechId JapanesePartOfSpeechId { get; private set; } = null!;
	public DependencyRelationshipId DependencyRelationshipId { get; private set; } = null!;
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	
	// NAVIGATION PROPERTIES
	// Bunsetsu
	public Bunsetsu.Bunsetsu Bunsetsu { get; private set; } = null!;

	// JapanesePartOfSpeech
	public JapanesePartOfSpeech JapanesePartOfSpeech { get; private set; } = null!;

	// DependencyRelationship
	public DependencyRelationship DependencyRelationship { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private Token() { }

	private Token(
		TokenId id,
		BunsetsuId bunsetsuId,
		string surface,
		string lemma,
		string reading,
		int headSequence,
		JapanesePartOfSpeechId japanesePartOfSpeechId,
		DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence)
		: base(id)
	{
		BunsetsuId = bunsetsuId;
		Surface = surface;
		Lemma = lemma;
		Reading = reading;
		HeadSequence = headSequence;
		JapanesePartOfSpeechId = japanesePartOfSpeechId;
		DependencyRelationshipId = dependencyRelationshipId;
		StartIndex = startIndex;
		EndIndex = endIndex;
		Sequence = sequence;
	}

	public static Result<Token> Create(
		BunsetsuId bunsetsuId,
		string surface,
		string lemma,
		string reading,
		int headSequence,
		JapanesePartOfSpeechId japanesePartOfSpeechId,
		DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence)
	{
		return Result.Success(new Token(
			TokenId.Create(),
			bunsetsuId,
			surface,
			lemma,
			reading,
			headSequence,
			japanesePartOfSpeechId,
			dependencyRelationshipId,
			startIndex,
			endIndex,
			sequence));
	}
}