using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Subtitle;

public sealed class SubtitleId : StronglyTypedId<Guid>
{
	private SubtitleId(Guid value) : base(value)
	{
	}

	public static SubtitleId Create() => new(Guid.NewGuid());

	public static SubtitleId From(Guid value) => new(value);
}