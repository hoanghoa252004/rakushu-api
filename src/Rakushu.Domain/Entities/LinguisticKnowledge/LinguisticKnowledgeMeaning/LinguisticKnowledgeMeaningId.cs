using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgeMeaning;

public sealed class LinguisticKnowledgeMeaningId : StronglyTypedId<Guid>
{
	private LinguisticKnowledgeMeaningId(Guid value) : base(value)
	{
	}

	public static LinguisticKnowledgeMeaningId Create() => new(Guid.NewGuid());

	public static LinguisticKnowledgeMeaningId From(Guid value) => new(value);
}