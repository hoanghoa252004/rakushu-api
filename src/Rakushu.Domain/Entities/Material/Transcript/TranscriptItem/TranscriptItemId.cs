using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;

public sealed class TranscriptItemId : StronglyTypedId<Guid>
{
	private TranscriptItemId(Guid value) : base(value)
	{
	}

	public static TranscriptItemId Create() => new(Guid.NewGuid());

	public static TranscriptItemId From(Guid value) => new(value);
}