using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;

public sealed class MessageEmbeddingId : StronglyTypedId<Guid>
{
	private MessageEmbeddingId(Guid value) : base(value)
	{
	}

	public static MessageEmbeddingId Create() => new(Guid.NewGuid());

	public static MessageEmbeddingId From(Guid value) => new(value);
}