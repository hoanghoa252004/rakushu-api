using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video;

public sealed class VideoId : StronglyTypedId<Guid>
{
	private VideoId(Guid value) : base(value)
	{
	}

	public static VideoId Create() => new(Guid.NewGuid());

	public static VideoId From(Guid value) => new(value);
}