using FluentValidation;

namespace Rakushu.Application.Usecases.Feature.UpdateFeature;

internal sealed class UpdateFeatureValidator : AbstractValidator<UpdateFeatureCommand>
{
	public UpdateFeatureValidator()
	{
		RuleFor(x => x.FeatureId)
			.NotEmpty()
			.WithMessage("Feature ID is required");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Feature name is required")
			.MaximumLength(50)
			.WithMessage("Feature name must not exceed 50 characters");
	}
}
