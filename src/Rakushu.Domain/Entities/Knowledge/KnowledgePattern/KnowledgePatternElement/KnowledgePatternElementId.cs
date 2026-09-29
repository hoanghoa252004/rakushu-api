using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;

public sealed class KnowledgePatternElementId : StronglyTypedId<Guid>
{
	private KnowledgePatternElementId(Guid value) : base(value)
	{
	}

	public static KnowledgePatternElementId Create() => new(Guid.NewGuid());

	public static KnowledgePatternElementId From(Guid value) => new(value);
}