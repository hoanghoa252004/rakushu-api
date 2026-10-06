using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern;

public sealed class LinguisticKnowledgePatternId : StronglyTypedId<Guid>
{
	private LinguisticKnowledgePatternId(Guid value) : base(value)
	{
	}

	public static LinguisticKnowledgePatternId Create() => new(Guid.NewGuid());

	public static LinguisticKnowledgePatternId From(Guid value) => new(value);
}
