using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsNoteId : StronglyTypedId<Guid>
{
    private SrsNoteId(Guid value) : base(value) { }

    public static SrsNoteId Create() => new(Guid.NewGuid());

    public static SrsNoteId From(Guid value) => new(value);
}
