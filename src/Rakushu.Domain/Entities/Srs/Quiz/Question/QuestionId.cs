using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.Quiz.Question;

public sealed class QuestionId : StronglyTypedId<Guid>
{
	private QuestionId(Guid value) : base(value)
	{
	}

	public static QuestionId Create() => new(Guid.NewGuid());
	public static QuestionId From(Guid value) => new(value);
}
