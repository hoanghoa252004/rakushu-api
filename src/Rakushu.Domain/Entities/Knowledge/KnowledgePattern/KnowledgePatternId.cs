using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern;

public sealed class KnowledgePatternId : StronglyTypedId<Guid>
{
	private KnowledgePatternId(Guid value) : base(value)
	{
	}

	public static KnowledgePatternId Create() => new(Guid.NewGuid());

	public static KnowledgePatternId From(Guid value) => new(value);
}
