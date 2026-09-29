using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

public sealed class Token : Entity<TokenId>
{
	public BunsetsuId BunsetsuId { get; private set; } = null!;
	public string Surface { get; private set; } = null!;
	public string Lemma { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public JapanesePartOfSpeechId JapanesePartOfSpeechId { get; private set; } = null!;
	public UniversalPartOfSpeechId UniversalPartOfSpeechId { get; private set; } = null!;
	public DependencyRelationshipId DependencyRelationshipId { get; private set; } = null!;
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	public int Sequence { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// Bunsetsu
	public Bunsetsu Bunsetsu { get; private set; } = null!;

	// JapanesePartOfSpeech
	public JapanesePartOfSpeech JapanesePartOfSpeech { get; private set; } = null!;

	// UniversalPartOfSpeech
	public UniversalPartOfSpeech UniversalPartOfSpeech { get; private set; } = null!;

	// DependencyRelationship
	public DependencyRelationship DependencyRelationship { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS

	private Token()
	{
	}

	private Token(
		TokenId id,
		BunsetsuId bunsetsuId,
		string surface,
		string lemma,
		string reading,
		JapanesePartOfSpeechId japanesePartOfSpeechId,
		UniversalPartOfSpeechId universalPartOfSpeechId,
		DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		BunsetsuId = bunsetsuId;
		Surface = surface;
		Lemma = lemma;
		Reading = reading;
		JapanesePartOfSpeechId = japanesePartOfSpeechId;
		UniversalPartOfSpeechId = universalPartOfSpeechId;
		DependencyRelationshipId = dependencyRelationshipId;
		StartIndex = startIndex;
		EndIndex = endIndex;
		Sequence = sequence;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}