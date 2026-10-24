using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsItemId : StronglyTypedId<Guid>
{
	private SrsItemId(Guid value) : base(value) { }

	public static SrsItemId Create() => new(Guid.NewGuid());

	public static SrsItemId From(Guid value) => new(value);
}
