using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Transcript;

public sealed class TranscriptId : StronglyTypedId<Guid>
{
	private TranscriptId(Guid value) : base(value)
	{
	}

	public static TranscriptId Create() => new(Guid.NewGuid());

	public static TranscriptId From(Guid value) => new(value);
}