using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

public sealed class SrsScheduleId : StronglyTypedId<Guid>
{
	private SrsScheduleId(Guid value) : base(value)
	{
	}

	public static SrsScheduleId Create() => new(Guid.NewGuid());
	public static SrsScheduleId From(Guid value) => new(value);
}
