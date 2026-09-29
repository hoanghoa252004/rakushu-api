using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaningDetail;

public sealed class KnowledgeMeaningDetailId : StronglyTypedId<Guid>
{
	private KnowledgeMeaningDetailId(Guid value) : base(value)
	{
	}

	public static KnowledgeMeaningDetailId Create() => new(Guid.NewGuid());

	public static KnowledgeMeaningDetailId From(Guid value) => new(value);
}