using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.QuestionType;

public sealed class QuestionTypeId : StronglyTypedId<Guid>
{
	private QuestionTypeId(Guid value) : base(value)
	{
	}

	public static QuestionTypeId Create() => new(Guid.NewGuid());
	public static QuestionTypeId From(Guid value) => new(value);
}
