using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;

public sealed class SrsItemDetailId : StronglyTypedId<Guid>
{
	private SrsItemDetailId(Guid value) : base(value)
	{
	}

	public static SrsItemDetailId Create() => new(Guid.NewGuid());
	public static SrsItemDetailId From(Guid value) => new(value);
}
