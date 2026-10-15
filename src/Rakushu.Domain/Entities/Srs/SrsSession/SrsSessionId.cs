using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsSessionId : StronglyTypedId<Guid>
{
    private SrsSessionId(Guid value) : base(value) { }

    public static SrsSessionId Create() => new(Guid.NewGuid());

    public static SrsSessionId From(Guid value) => new(value);
}
