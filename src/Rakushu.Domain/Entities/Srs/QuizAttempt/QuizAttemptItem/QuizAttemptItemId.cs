using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttemptItemId : StronglyTypedId<Guid>
{
    private QuizAttemptItemId(Guid value) : base(value) { }

    public static QuizAttemptItemId Create() => new(Guid.NewGuid());

    public static QuizAttemptItemId From(Guid value) => new(value);
}
