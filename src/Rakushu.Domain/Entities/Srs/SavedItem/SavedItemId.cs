using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public sealed class SavedItemId : StronglyTypedId<Guid>
{
	private SavedItemId(Guid value) : base(value)
	{
	}

	public static SavedItemId Create() => new(Guid.NewGuid());
	public static SavedItemId From(Guid value) => new(value);
}
