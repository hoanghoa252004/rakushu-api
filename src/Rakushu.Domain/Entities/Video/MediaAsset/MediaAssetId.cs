using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.MediaAsset;

public sealed class MediaAssetId : StronglyTypedId<Guid>
{
	private MediaAssetId(Guid value) : base(value)
	{
	}

	public static MediaAssetId Create() => new(Guid.NewGuid());

	public static MediaAssetId From(Guid value) => new(value);
}