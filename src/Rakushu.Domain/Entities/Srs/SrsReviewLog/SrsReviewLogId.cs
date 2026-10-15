using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsReviewLogId : StronglyTypedId<Guid>
{
    private SrsReviewLogId(Guid value) : base(value) { }

    public static SrsReviewLogId Create() => new(Guid.NewGuid());

    public static SrsReviewLogId From(Guid value) => new(value);
}
