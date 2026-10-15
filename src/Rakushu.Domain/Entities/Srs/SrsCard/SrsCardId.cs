using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsCardId : StronglyTypedId<Guid>
{
    private SrsCardId(Guid value) : base(value) { }

    public static SrsCardId Create() => new(Guid.NewGuid());

    public static SrsCardId From(Guid value) => new(value);
}
