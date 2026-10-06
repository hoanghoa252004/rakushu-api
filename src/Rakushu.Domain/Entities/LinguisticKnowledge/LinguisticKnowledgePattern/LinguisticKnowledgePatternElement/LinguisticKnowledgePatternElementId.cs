using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternElement;

public sealed class LinguisticKnowledgePatternElementId : StronglyTypedId<Guid>
{
	private LinguisticKnowledgePatternElementId(Guid value) : base(value)
	{
	}

	public static LinguisticKnowledgePatternElementId Create() => new(Guid.NewGuid());

	public static LinguisticKnowledgePatternElementId From(Guid value) => new(value);
}