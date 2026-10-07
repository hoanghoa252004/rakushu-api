using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

public sealed class UniversalPartOfSpeech : Linguistic<UniversalPartOfSpeechId>
{
	// NAVIGATION PROPERTIES
	// Tokens
	private readonly List<Token> _tokens = new();
	public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private UniversalPartOfSpeech()
	{
	}

	private UniversalPartOfSpeech(
		UniversalPartOfSpeechId id,
		string code,
		string name,
		string vietnameseName,
		string? description)
		: base(
			id,
			code,
			name,
			vietnameseName,
			description)
	{
	}

	public static UniversalPartOfSpeech Create(
		UniversalPartOfSpeechId id,
		string code,
		string name,
		string vietnameseName,
		string? description = null)
	{
		return new UniversalPartOfSpeech(
			id,
			code,
			name,
			vietnameseName,
			description);
	}
}