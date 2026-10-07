using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
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

}