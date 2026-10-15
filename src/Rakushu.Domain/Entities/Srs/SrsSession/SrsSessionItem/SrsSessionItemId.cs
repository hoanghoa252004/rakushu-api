using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsSessionItemId : StronglyTypedId<Guid>
{
    private SrsSessionItemId(Guid value) : base(value) { }

    public static SrsSessionItemId Create() => new(Guid.NewGuid());

    public static SrsSessionItemId From(Guid value) => new(value);
}
