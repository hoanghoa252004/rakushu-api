using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;

public sealed class LinguisticKnowledgeId : StronglyTypedId<Guid>
{
	private LinguisticKnowledgeId(Guid value) : base(value)
	{
	}

	public static LinguisticKnowledgeId Create() => new(Guid.NewGuid());

	public static LinguisticKnowledgeId From(Guid value) => new(value);
}