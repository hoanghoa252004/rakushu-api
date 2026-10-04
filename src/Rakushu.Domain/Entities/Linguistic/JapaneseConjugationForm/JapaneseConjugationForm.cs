using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;
using Rakushu.Domain.Entities.Linguistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;

public sealed class JapaneseConjugationForm : Linguistic<JapaneseConjugationFormId>
{
	// NAVIGATION PROPERTIES
	// KnowledgePatternElements
	private readonly List<KnowledgePatternElement> _knowledgePatternElements = new();
	public IReadOnlyCollection<KnowledgePatternElement> KnowledgePatternElements => _knowledgePatternElements.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private JapaneseConjugationForm()
	{
	}

	private JapaneseConjugationForm(
		JapaneseConjugationFormId id,
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

	public static JapaneseConjugationForm Create(
		JapaneseConjugationFormId id,
		string code,
		string name,
		string vietnameseName,
		string? description = null)
	{
		return new JapaneseConjugationForm(
			id,
			code,
			name,
			vietnameseName,
			description);
	}
}