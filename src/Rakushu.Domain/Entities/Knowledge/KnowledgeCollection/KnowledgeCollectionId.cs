using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

public sealed class KnowledgeCollectionId : StronglyTypedId<Guid>
{
	private KnowledgeCollectionId(Guid value) : base(value)
	{
	}

	public static KnowledgeCollectionId Create() => new(Guid.NewGuid());

	public static KnowledgeCollectionId From(Guid value) => new(value);
}