using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

public sealed class JapaneseConjugationForm : Linguistic<JapaneseConjugationFormId>
{
	// NAVIGATION PROPERTIES
	// KnowledgePatternElements
	private readonly List<LinguisticKnowledgePatternElement> _knowledgePatternElements = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternElement> KnowledgePatternElements => _knowledgePatternElements.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private JapaneseConjugationForm()
	{
	}

	private JapaneseConjugationForm(
		JapaneseConjugationFormId id,
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

	public static JapaneseConjugationForm Create(
		JapaneseConjugationFormId id,
		string code,
		string name,
		string japaneseName,
		string? description = null)
	{
		return new JapaneseConjugationForm(
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