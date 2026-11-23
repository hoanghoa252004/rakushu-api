using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Message;

public sealed class MessageId : StronglyTypedId<Guid>
{
	private MessageId(Guid value) : base(value)
	{
	}

	public static MessageId Create() => new(Guid.NewGuid());

	public static MessageId From(Guid value) => new(value);
}