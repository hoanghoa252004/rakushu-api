using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Attachment;

public sealed class AttachmentId : StronglyTypedId<Guid>
{
	private AttachmentId(Guid value) : base(value)
	{
	}

	public static AttachmentId Create() => new(Guid.NewGuid());

	public static AttachmentId From(Guid value) => new(value);
}