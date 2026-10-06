using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternElement;

public sealed class LinguisticKnowledgePatternElement
	: Entity<LinguisticKnowledgePatternElementId>
{
	public LinguisticKnowledgePatternId KnowledgePatternId { get; private set; } = null!;
	public int Sequence { get; private set; }
	public LinguisticKnowledgePatternElementRole Role { get; private set; }
	public LinguisticKnowledgePatternElementScope Scope { get; private set; }


	// Matching constraints
	public string? Surface { get; private set; }
	public string? Lemma { get; private set; }
	public UniversalPartOfSpeechId? UniversalPartOfSpeechId { get; private set; }
	public JapanesePartOfSpeechId? JapanesePartOfSpeechId { get; private set; }
	public JapaneseConjugationFormId? JapaneseConjugationFormId { get; private set; }
	public string? Particle { get; private set; }


	// Slot extraction
	public string? SlotName { get; private set; } 
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// KnowledgePattern
	public LinguisticKnowledgePattern KnowledgePattern { get; private set; } = null!;


	// Outgoing dependency relations
	private readonly List<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> _outgoingRelations = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> OutgoingRelations
		=> _outgoingRelations.AsReadOnly();

	// Incoming dependency relations
	private readonly List<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> _incomingRelations = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> IncomingRelations
		=> _incomingRelations.AsReadOnly();

	// UniversalPartOfSpeech
	public UniversalPartOfSpeech? UniversalPartOfSpeech { get; private set; }

	// JapanesePartOfSpeech
	public JapanesePartOfSpeech? JapanesePartOfSpeech { get; private set; }

	// JapaneseConjugationForm
	public JapaneseConjugationForm? JapaneseConjugationForm { get; private set; }

	// CONSTRUCTORS & FACTORY METHODS
	private LinguisticKnowledgePatternElement()
	{
	}

	private LinguisticKnowledgePatternElement(
		LinguisticKnowledgePatternElementId id,
		LinguisticKnowledgePatternId knowledgePatternId,
		int sequence,
		LinguisticKnowledgePatternElementRole role,
		LinguisticKnowledgePatternElementScope scope,
		string? surface,
		string? lemma,
		UniversalPartOfSpeechId? universalPartOfSpeechId,
		JapanesePartOfSpeechId? japanesePartOfSpeechId,
		JapaneseConjugationFormId? japaneseConjugationFormId,
		string? particle,
		string? slotName,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		KnowledgePatternId = knowledgePatternId;
		Sequence = sequence;
		Role = role;
		Scope = scope;
		Surface = surface;
		Lemma = lemma;
		UniversalPartOfSpeechId = universalPartOfSpeechId;
		JapanesePartOfSpeechId = japanesePartOfSpeechId;
		JapaneseConjugationFormId = japaneseConjugationFormId;
		Particle = particle;
		SlotName = slotName;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}