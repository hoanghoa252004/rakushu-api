using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

public sealed class ContentKnowledgeCollectionId : StronglyTypedId<Guid>
{
	private ContentKnowledgeCollectionId(Guid value) : base(value)
	{
	}

	public static ContentKnowledgeCollectionId Create() => new(Guid.NewGuid());

	public static ContentKnowledgeCollectionId From(Guid value) => new(value);
}