using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;

public sealed class SrsImmersionBoostLogId : StronglyTypedId<Guid>
{
	private SrsImmersionBoostLogId(Guid value) : base(value)
	{
	}

	public static SrsImmersionBoostLogId Create() => new(Guid.NewGuid());
	public static SrsImmersionBoostLogId From(Guid value) => new(value);
}
