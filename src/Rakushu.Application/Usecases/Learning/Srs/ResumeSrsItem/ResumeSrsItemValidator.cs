using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

internal sealed class ResumeSrsItemValidator : AbstractValidator<ResumeSrsItemCommand>
{
	public ResumeSrsItemValidator()
	{
		RuleFor(x => x.ItemId)
			.NotEmpty().WithMessage("ItemId is required.");
	}
}
