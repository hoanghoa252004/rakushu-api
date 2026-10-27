using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.BankQuestion;

public sealed class BankQuestionId : StronglyTypedId<Guid>
{
	private BankQuestionId(Guid value) : base(value)
	{
	}

	public static BankQuestionId Create() => new(Guid.NewGuid());
	public static BankQuestionId From(Guid value) => new(value);
}
