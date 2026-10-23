using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnitElement;

public sealed class KnowledgeUnitElement : Entity<KnowledgeUnitElementId>
{
	public int Sequence { get; private set; }
	public KnowledgeUnitId KnowledgeUnitId { get; private set; } = null!;
	public string Surface { get; private set; } = null!;
	public string Lemma { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public int HeadSequence { get; private set; }
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	public JapanesePartOfSpeechId JapanesePartOfSpeechId { get; private set; } = null!;
	public DependencyRelationshipId DependencyRelationshipId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	// KnowledgeUnit
	public KnowledgeUnit.KnowledgeUnit KnowledgeUnit { get; private set; } = null!;

	//JapanesePartOfSpeech
	public JapanesePartOfSpeech.JapanesePartOfSpeech JapanesePartOfSpeech { get; private set; } = null!;

	// DependencyRelationship
	public DependencyRelationship.DependencyRelationship DependencyRelationship { get; private set; } = null!;

	private KnowledgeUnitElement()
	{
	}

	private KnowledgeUnitElement(
		KnowledgeUnitElementId id,
		KnowledgeUnitId knowledgeUnitId,
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
		KnowledgeUnitId = knowledgeUnitId;
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

	public static Result<KnowledgeUnitElement> Create(
		KnowledgeUnitId knowledgeUnitId,
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
		return Result.Success(new KnowledgeUnitElement(
			KnowledgeUnitElementId.Create(),
			knowledgeUnitId,
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