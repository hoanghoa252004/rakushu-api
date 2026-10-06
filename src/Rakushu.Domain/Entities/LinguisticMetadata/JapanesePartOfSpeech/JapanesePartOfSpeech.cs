using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternElement;
using Rakushu.Domain.Entities.LinguisticMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

public sealed class JapanesePartOfSpeech : Linguistic<JapanesePartOfSpeechId>
{
	// NAVIGATION PROPERTIES
	// Tokens
	private readonly List<Token> _tokens = new();
	public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

	// KnowledgePatternElements
	private readonly List<LinguisticKnowledgePatternElement> _knowledgePatternElements = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternElement> KnowledgePatternElements => _knowledgePatternElements.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private JapanesePartOfSpeech()
	{
	}

	private JapanesePartOfSpeech(
		JapanesePartOfSpeechId id,
		string code,
		string name,
		string japaneseName,
		string? description)
		: base(
			id,
			code,
			name,
			japaneseName,
			description)
	{
	}

	public static JapanesePartOfSpeech Create(
		JapanesePartOfSpeechId id,
		string code,
		string name,
		string japaneseName,
		string? description = null)
	{
		return new JapanesePartOfSpeech(
			id,
			code,
			name,
			japaneseName,
			description);
	}

	public void Update(
		string name,
		string japaneseName,
		string? description)
	{
		base.Update(name, japaneseName, description);
	}
}