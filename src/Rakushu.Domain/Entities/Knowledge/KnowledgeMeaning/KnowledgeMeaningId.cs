using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

public sealed class KnowledgeMeaningId : StronglyTypedId<Guid>
{
	private KnowledgeMeaningId(Guid value) : base(value)
	{
	}

	public static KnowledgeMeaningId Create() => new(Guid.NewGuid());

	public static KnowledgeMeaningId From(Guid value) => new(value);
}