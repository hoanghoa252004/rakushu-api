using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Conversation;

public sealed class ConversationId : StronglyTypedId<Guid>
{
	private ConversationId(Guid value) : base(value)
	{
	}

	public static ConversationId Create() => new(Guid.NewGuid());

	public static ConversationId From(Guid value) => new(value);
}