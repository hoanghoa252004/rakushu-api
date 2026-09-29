using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternRelation;

public sealed class KnowledgePatternRelationId : StronglyTypedId<Guid>
{
	private KnowledgePatternRelationId(Guid value) : base(value)
	{
	}

	public static KnowledgePatternRelationId Create() => new(Guid.NewGuid());

	public static KnowledgePatternRelationId From(Guid value) => new(value);
}