using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt;

public sealed class QuizAttemptId : StronglyTypedId<Guid>
{
	private QuizAttemptId(Guid value) : base(value)
	{
	}

	public static QuizAttemptId Create() => new(Guid.NewGuid());
	public static QuizAttemptId From(Guid value) => new(value);
}
