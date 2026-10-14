using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

public sealed class KnowledgeUnitId : StronglyTypedId<Guid>
{
	private KnowledgeUnitId(Guid value) : base(value)
	{
	}

	public static KnowledgeUnitId Create() => new(Guid.NewGuid());

	public static KnowledgeUnitId From(Guid value) => new(value);
}