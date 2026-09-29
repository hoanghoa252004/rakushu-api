using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Subtitle.SubtitleSegment;

public sealed class SubtitleSegmentId : StronglyTypedId<Guid>
{
	private SubtitleSegmentId(Guid value) : base(value)
	{
	}

	public static SubtitleSegmentId Create() => new(Guid.NewGuid());

	public static SubtitleSegmentId From(Guid value) => new(value);
}