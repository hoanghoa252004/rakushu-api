using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;

public sealed class KnowledgePatternElement
	: Entity<KnowledgePatternElementId>
{
	public KnowledgePatternId KnowledgePatternId { get; private set; } = null!;
	public int Sequence { get; private set; }
	public KnowledgePatternElementRole Role { get; private set; }
	public KnowledgePatternElementScope Scope { get; private set; }


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
	public KnowledgePattern KnowledgePattern { get; private set; } = null!;


	// Outgoing dependency relations
	private readonly List<KnowledgePatternRelation.KnowledgePatternRelation> _outgoingRelations = new();
	public IReadOnlyCollection<KnowledgePatternRelation.KnowledgePatternRelation> OutgoingRelations
		=> _outgoingRelations.AsReadOnly();

	// Incoming dependency relations
	private readonly List<KnowledgePatternRelation.KnowledgePatternRelation> _incomingRelations = new();
	public IReadOnlyCollection<KnowledgePatternRelation.KnowledgePatternRelation> IncomingRelations
		=> _incomingRelations.AsReadOnly();

	// UniversalPartOfSpeech
	public UniversalPartOfSpeech? UniversalPartOfSpeech { get; private set; }

	// JapanesePartOfSpeech
	public JapanesePartOfSpeech? JapanesePartOfSpeech { get; private set; }

	// JapaneseConjugationForm
	public JapaneseConjugationForm? JapaneseConjugationForm { get; private set; }

	// CONSTRUCTORS & FACTORY METHODS
	private KnowledgePatternElement()
	{
	}

	private KnowledgePatternElement(
		KnowledgePatternElementId id,
		KnowledgePatternId knowledgePatternId,
		int sequence,
		KnowledgePatternElementRole role,
		KnowledgePatternElementScope scope,
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