using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SuspendSrsItem;

internal sealed class SuspendSrsItemValidator : AbstractValidator<SuspendSrsItemCommand>
{
	public SuspendSrsItemValidator()
	{
		RuleFor(x => x.ItemId)
			.NotEmpty().WithMessage("ItemId is required.");
	}
}
