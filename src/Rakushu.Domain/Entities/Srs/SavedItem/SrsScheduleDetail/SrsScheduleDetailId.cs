using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

public sealed class SrsScheduleDetailId : StronglyTypedId<Guid>
{
	private SrsScheduleDetailId(Guid value) : base(value) { }

	public static SrsScheduleDetailId Create() => new(Guid.NewGuid());
	public static SrsScheduleDetailId From(Guid value) => new(value);
}
