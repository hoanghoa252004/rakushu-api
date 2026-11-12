using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.Quiz;

public sealed class QuizId : StronglyTypedId<Guid>
{
	private QuizId(Guid value) : base(value)
	{
	}

	public static QuizId Create() => new(Guid.NewGuid());
	public static QuizId From(Guid value) => new(value);
}
