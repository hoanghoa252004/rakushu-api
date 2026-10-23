using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnitElement;

public sealed class KnowledgeUnitElementId : StronglyTypedId<Guid>
{
	private KnowledgeUnitElementId(Guid value) : base(value)
	{
	}

	public static KnowledgeUnitElementId Create() => new(Guid.NewGuid());

	public static KnowledgeUnitElementId From(Guid value) => new(value);
}