using FluentValidation;

namespace Rakushu.Application.Usecases.Subscription.Feature.CreateFeature;

internal sealed class CreateFeatureValidator : AbstractValidator<CreateFeatureCommand>
{
	public CreateFeatureValidator()
	{
		RuleFor(x => x.Code)
			.NotEmpty()
			.WithMessage("Feature code is required")
			.MaximumLength(50)
			.WithMessage("Feature code must not exceed 50 characters");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Feature name is required")
			.MaximumLength(50)
			.WithMessage("Feature name must not exceed 50 characters");

		RuleFor(x => x.IsActive)
			.NotNull().WithMessage("Feature IsActive is required");
	}
}
