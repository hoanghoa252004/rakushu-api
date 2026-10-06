using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternRelation;

public sealed class LinguisticKnowledgePatternRelationId : StronglyTypedId<Guid>
{
	private LinguisticKnowledgePatternRelationId(Guid value) : base(value)
	{
	}

	public static LinguisticKnowledgePatternRelationId Create() => new(Guid.NewGuid());

	public static LinguisticKnowledgePatternRelationId From(Guid value) => new(value);
}